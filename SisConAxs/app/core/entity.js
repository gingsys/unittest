export class Entity {
    static fromList(list) {
        return list.map(obj => {
            let item = new this();
            item.fromDTO(obj);
            return item;
        });
    }
    static fromObject(obj) {
        let item = new this();
        item.fromDTO(obj);
        return item;
    }

    static __copy(source, dest) {
        for (let attr in dest) {
            let value = source[attr];
            if (value !== undefined && typeof dest[attr] !== 'function') {
                // if (Array.isArray(dest[attr]) && Array.isArray(source[attr])) {
                //     dest[attr] = [];
                //     source[attr].forEach(x => {
                //         if(x instanceof Entity) {

                //         }
                //     });
                //     // forEach((el, index) => __copy(source[attr], dest[index]));
                // } else
                if (dest[attr] instanceof Entity) {
                    dest[attr].fromDTO(source[attr]);
                } else {
                    dest[attr] = value;
                }
            }
        }
    }

    fromDTO(dto) {
        Entity.__copy(dto, this);
    }
    toDTO() {
        return JSON.parse(JSON.stringify(this));
    }

    copy() {
        let entity = new this.constructor();
        entity.fromDTO(this.toDTO());
        return entity;
    }

    __deepEqual(object1, object2) {
        const keys1 = Object.keys(object1);
        const keys2 = Object.keys(object2);
      
        if (keys1.length !== keys2.length) {
          return false;
        }
      
        for (const key of keys1) {
          const val1 = object1[key];
          const val2 = object2[key];
          const areObjects = isObject(val1) && isObject(val2);
          if (
            areObjects && !this.__deepEqual(val1, val2) ||
            !areObjects && val1 !== val2
          ) {
            return false;
          }
        }
      
        return true;
        function isObject(object) {
            return object != null && typeof object === 'object';
        }
    }
    equal(dest) {
        return this.__deepEqual(this, dest);
    }
}