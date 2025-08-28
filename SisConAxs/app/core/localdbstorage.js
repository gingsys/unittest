import { Entity } from "./entity.js";

export class LocalDbStorage {

    constructor(entityClass, databaseName, version, name, {keyPath, autoIncrement, alias, upgradeHandler} = {}) {
        if (!('indexedDB' in window)) {
            throw '> LocalDbStorage: [Error] >> This browser doesn\'t support IndexedDB';
        }

        if(!!entityClass && entityClass.prototype instanceof Entity) {
            this.__entityClass = entityClass;
            this.__databaseName = databaseName;
            this.__databaseVersion = version;
            this.__storageName = `${name}${!!alias && typeof alias == 'string' && alias.trim() !== ''? `<<${alias}>>` : ''}`;
            this.__storageConfig = {keyPath, autoIncrement};
            this.__upgradeHandler = !!upgradeHandler && typeof upgradeHandler == typeof(Function)? upgradeHandler.bind(this) : undefined;
        } else {
            throw `> LocalDbStorage: [Error] >> Not valid entity class`;
        }
    }

    static registerStore(localDbStorage) {
        if(!!localDbStorage && localDbStorage instanceof LocalDbStorage) {
            this.__register = this.__register || [];
            if(this.__register.some(ls => ls.databaseName == localDbStorage.databaseName && ls.storageName == localDbStorage.storageName))
                throw new Error(`> LocalDbStorage.registerStore: [Error] >> ${localDbStorage.databaseName} -> ${localDbStorage.storageName} has been registred.`);
            this.__register.push(localDbStorage);
            return;
        }
        throw new Error(`> LocalDbStorage.registerStore: [Error] >> localDbStorage isn't instance of LocalDbStorage`);
    }
    static getStore(databaseName, name, alias) {
        this.__register = this.__register || [];
        let storageName = `${name}${!!alias && typeof alias == 'string' && alias.trim() !== ''? `<<${alias}>>` : ''}`;
        let store = this.__register.find(localStore => localStore.databaseName == databaseName && localStore.storageName == storageName);
        if(store === undefined)
            throw new Error(`> LocalDbStorage.getStore: [Error] >> Store not found ${databaseName} -> ${storageName}`);
        return store;
    }

    static initDatabase(databaseName, version) {
        new Promise(function(resolve, reject) {
            const request = LocalDbStorage.__openDatabaseRequest(databaseName, version);
            request.onsuccess = (event) => {
                let db = event.target.result;
                db.onerror = function(event) {
                    // Generic error handler for all errors targeted at this database's requests!
                    console.error(`> LocalDbStorage: [Error] ${databaseName} -> code (${event.target.errorCode}) >>`, event);
                    throw event;
                };
                if(!!resolve) resolve();
            };
            request.onerror = (event) => {
                console.error(`> LocalDbStorage.initDatabase: [Error] ${databaseName} >>`, event);
                if(!!reject) reject();
            }
        });
    }

    static __openDatabaseRequest(databaseName, version) {
        const request = window.indexedDB.open(databaseName, version);
        request.onupgradeneeded = (event) => {
            let db = event.target.result;
            let regiteredStorages = (LocalDbStorage.__register || [])
                                    .filter(localStore => localStore.databaseName == databaseName);
            
            // delete object stores not included in register
            Array.from(db.objectStoreNames)
                 .filter(storageName => !regiteredStorages.find(s => s.storageName == storageName))
                 .forEach(storageName => {
                    db.deleteObjectStore(storageName);
                 });

            // create object stores
            regiteredStorages.forEach(localStore => {
                if(!!localStore.upgradeHandler) {
                    localStore.upgradeHandler(db, event);
                } else {
                    if(db.objectStoreNames.contains(localStore.storageName)) {
                        db.deleteObjectStore(localStore.storageName, localStore.storageConfig);
                    }
                    db.createObjectStore(localStore.storageName, localStore.storageConfig);
                }
            });
        }
        return request;
    }

    __openDatabase(success) {
        const request = LocalDbStorage.__openDatabaseRequest(this.__databaseName, this.__databaseVersion);
        request.onsuccess = (event) => {
            let db = event.target.result;
            db.onerror = function(event) {
                // Generic error handler for all errors targeted at this database's requests!
                console.error(`> LocalDbStorage: [Error] ${this.__databaseName} -> code (${event.target.errorCode}) >>`, event);
                throw event;
            };
            success(db);
        };
        request.onerror = (event) => {
            console.error(`> LocalDbStorage: [Error] ${this.__databaseName} >>`, event);
            throw event;
        }
        return request;
    }

    get databaseName() {
        return this.__databaseName;
    }
    get databaseVersion() {
        return this.__databaseVersion;
    }
    get storageName() {
        return this.__storageName;
    }
    get storageConfig() {
        return this.__storageConfig;
    }
    get upgradeHandler() {
        return this.__upgradeHandler;
    }
    get entityClass() {
        return this.__entityClass;
    }

    get storage() {
        return new Promise((resolve, reject) => {
            this.__openDatabase((db) => {
                if(db.objectStoreNames.contains(this.__storageName)) {
                    let store = db.transaction(this.__storageName, 'readwrite').objectStore(this.__storageName);
                    resolve(store);
                } else {
                    throw new Error(`> LocalDbStorage: [Error] ${this.__databaseName} >> ObjectStore '${this.__storageName}' not found`);
                }
            });
        });
    }

    get syncDate() {
        let value = window.localStorage[`SyncDate<${this.__storageName}>`];
        if(value != '') 
            return new Date(window.localStorage[`SyncDate<${this.__storageName}>`]);
        return null;
    }
    __updateSyncDate() {
        window.localStorage[`SyncDate<${this.__storageName}>`] = new Date();
    }

    getAll() {
        return new Promise((resolve, reject) => {
            this.storage.then(storage => {
                const request = storage.getAll();
                request.onsuccess = (event) => {
                    const data = this.__entityClass.fromList(event.target.result || []);
                    console.log(`> LocalDbStorage.getAll: ${this.databaseName} -> ${this.storageName} >>`, data);
                    resolve(data);
                };
                request.onerror = (event) => {
                    console.error(`> LocalDbStorage.getAll: [Error] ${this.databaseName} -> ${this.storageName} >> ${event.target.error}`, event);
                    reject(event);
                }
            });
        });
    }

    get(key) {
        return new Promise((resolve, reject) => {
            this.storage.then(storage => {
                const request = storage.get(key);
                request.onsuccess = (event) => {
                    const data = this.__entityClass.fromObject(event.target.result);
                    console.log(`> LocalDbStorage.get: ${this.databaseName} -> ${this.storageName} -> ${key} >>`, data);
                    resolve(data);
                }
                request.onerror = (event) => {
                    console.error(`> LocalDbStorage.get: [Error] ${this.databaseName} -> ${this.storageName} -> ${key} >> value not found ${event.target.error}`, event);
                    reject(event);
                }
            });
        });
    }

    query(searchFunction) {
        return new Promise((resolve, reject) => {
            this.storage.then(storage => {
                const request = storage.getAll();
                request.onsuccess = (event) => {
                    const data = this.__entityClass.fromList((event.target.result || []).filter(searchFunction));
                    console.log(`> LocalDbStorage.query: ${this.databaseName} -> ${this.storageName} >>`, data);
                    resolve(data);
                };
                request.onerror = (event) => {
                    console.error(`> LocalDbStorage.query: [Error] ${this.databaseName} -> ${this.storageName} >> ${event.target.error}`, event);
                    reject(event);
                }
            });
        });
    }

    add(value) {
        if(value instanceof this.__entityClass) {
            return new Promise((resolve, reject) => {
                this.storage.then(storage => {
                    const request = storage.add(value);
                    request.onsuccess = (event) => {
                        console.log(`> LocalDbStorage.add: ${this.databaseName} -> ${this.storageName} >>`, value);
                        resolve(event.target, storage);
                    }
                    request.onerror = (event) => {
                        console.error(`> LocalDbStorage.add: [Error] ${this.databaseName} -> ${this.storageName} >> ${event.target.error}`, event);
                        reject(event);
                    }
                });
            });
        }
        throw `> LocalDbStorage.add: [Error] ${this.databaseName} -> ${this.storageName} >> value ins't a instance of ${this.__entityClass.name}`;
    }

    put(value) {
        if(value instanceof this.__entityClass) {
            return new Promise((resolve, reject) => {
                this.storage.then(storage => {
                    const request = storage.put(value);
                    request.onsuccess = (event) => {
                        console.log(`> LocalDbStorage.put: ${this.databaseName} -> ${this.storageName} >>`, value);
                        resolve(event.target, storage);
                    }
                    request.onerror = (event) => {
                        console.error(`> LocalDbStorage.put: [Error] ${this.databaseName} -> ${this.storageName} >> ${event.target.error}`, event);
                        reject(event);
                    }
                });
            });
        }
        throw `> LocalDbStorage.put: [Error] ${this.databaseName} -> ${this.storageName} >> value ins't a instance of ${this.__entityClass.name}`;
    }

    addAll(values) {
        if(Array.isArray(values)) {
            return new Promise((resolve, reject) => {
                this.storage.then(storage => {
                    values.forEach(value => {
                        if(value instanceof this.__entityClass) {
                            const request = storage.add(value);
                            request.onerror = (event) => {
                                console.error(`> LocalDbStorage.addAll: [Error] ${this.databaseName} -> ${this.storageName} >> error on add value ${event.target.error}`, event);
                                reject(event);
                            }
                        } else {
                            throw `> LocalDbStorage.addAll: [Error] ${this.databaseName} -> ${this.storageName} >> value ins't a instance of ${this.__entityClass.name}`;
                        }
                    });
                    console.log(`> LocalDbStorage.addAll: ${this.databaseName} -> ${this.storageName} >>`, values);
                    resolve(this);
                })
                .catch(e => reject(e));
            });
        }
        throw `> LocalDbStorage.addAll: [Error] ${this.databaseName} -> ${this.storageName} >> Values ins't a Array`;
    }

    sync(values) {
        if(Array.isArray(values)) {
            return new Promise((resolve, reject) => {            
                this.storage.then(storage => {
                    storage.clear();
                    values.forEach(value => {
                        if(value instanceof this.__entityClass) {
                            const request = storage.add(value);
                            request.onerror = (event) => {
                                console.error(`> LocalDbStorage.sync: [Error] ${this.databaseName} -> ${this.storageName} >> Error on add value ${event.target.error}`, event);
                                reject(event);
                            }
                        } else {
                            throw `> LocalDbStorage.sync: [Error] >> Value ins't a instance of ${this.__entityClass.name}`;
                        }
                    });
                    this.__updateSyncDate();
                    console.log(`> LocalDbStorage.sync: ${this.databaseName} -> ${this.storageName} >>`, values);
                    resolve(this);
                })
                .catch(e => reject(e));
            });
        }
        throw `> LocalDbStorage.sync: [Error] ${this.databaseName} -> ${this.storageName} >> Values ins't a Array`;
    }

    delete(key) {
        return new Promise((resolve, reject) => {
            this.storage.then(storage => {
                const request = storage.delete(key);
                request.onsuccess = (event) => {
                    console.log(`> LocalDbStorage.delete: ${this.databaseName} -> ${this.storageName} -> ${key}`);
                    resolve(this);
                }
                request.onerror = (event) => {
                    console.error(`> LocalDbStorage.delete: [Error] ${this.databaseName} -> ${this.storageName} -> ${key} >> value not found ${event.target.error}`, event);
                    reject(event);
                }
            });
        });
    }

    clear() {
        return new Promise((resolve, reject) => {
            this.storage.then(storage => {
                const request = storage.clear();
                request.onsuccess = (event) => {
                    console.log(`> LocalDbStorage.clear: ${this.databaseName} -> ${this.storageName}`);
                    resolve(this);
                }
                request.onerror = (event) => {
                    console.error(`> LocalDbStorage.clear: [Error] ${this.databaseName} -> ${this.storageName} >> ${event.target.error}`, event);
                    reject(event);
                }
            });
        });
    }
}