export const msalConfig = {
    auth: {
        clientId: "546d6990-a3bd-4b1e-9dfd-d55977c45207",
        authority: "https://login.microsoftonline.com/65bb3aad-376d-42f5-83f9-9beaea39fdc4",
        redirectUri: `${window.location.origin}/api/account/`,
    },
    cache: {
        cacheLocation: "sessionStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
    }
};

// Add here scopes for id token to be used at MS Identity Platform endpoints.
export const loginRequest = {
    scopes: ["openid", "profile", "User.Read", "Directory.Read.All"]
};

// Add here scopes for access token to be used at MS Graph API endpoints.
export const tokenRequest = {
    scopes: ["Directory.Read.All"]
};