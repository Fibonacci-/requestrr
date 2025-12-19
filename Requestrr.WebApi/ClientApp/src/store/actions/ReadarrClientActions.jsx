export const READARR_SET_CLIENT = "bookClients:set_readarr_client";
export const READARR_LOAD_PATHS = "bookClients:load_readarr_paths";
export const READARR_SET_PATHS = "bookClients:set_readarr_paths";
export const READARR_LOAD_PROFILES = "bookClients:load_readarr_profiles";
export const READARR_SET_PROFILES = "bookClients:set_readarr_profiles";
export const READARR_LOAD_METADATA_PROFILES = "bookClients:load_readarr_metadata_profiles";
export const READARR_SET_METADATA_PROFILES = "bookClients:set_readarr_metadata_profiles";
export const READARR_LOAD_TAGS = "bookClients:load_readarr_tags";
export const READARR_SET_TAGS = "bookClients:set_readarr_tags";


export function setReadarrClient(settings) {
    return {
        type: READARR_SET_CLIENT,
        payload: settings
    };
};


export function isLoadingReadarrPaths(isLoading) {
    return {
        type: READARR_LOAD_PATHS,
        payload: isLoading
    };
};


export function setReadarrPaths(readarrPaths) {
    return {
        type: READARR_SET_PATHS,
        payload: readarrPaths
    };
};


export function isLoadingReadarrProfiles(isLoading) {
    return {
        type: READARR_LOAD_PROFILES,
        payload: isLoading
    };
};


export function setReadarrProfiles(readarrProfiles) {
    return {
        type: READARR_SET_PROFILES,
        payload: readarrProfiles
    };
};


export function isLoadingReadarrMetadataProfiles(isLoading) {
    return {
        type: READARR_LOAD_METADATA_PROFILES,
        payload: isLoading
    };
};


export function setReadarrMetadataProfiles(readarrMetadataProfiles) {
    return {
        type: READARR_SET_METADATA_PROFILES,
        payload: readarrMetadataProfiles
    };
};


export function isLoadingReadarrTags(isLoading) {
    return {
        type: READARR_LOAD_TAGS,
        payload: isLoading
    };
};


export function setReadarrTags(readarrTags) {
    return {
        type: READARR_SET_TAGS,
        payload: readarrTags
    };
};


export function setReadarrConnectionSettings(connectionSettings) {
    return (dispatch, getState) => {
        const state = getState();

        let readarr = {
            ...state.books.readarr,
            hostname: connectionSettings.hostname,
            baseUrl: connectionSettings.baseUrl,
            port: connectionSettings.port,
            apiKey: connectionSettings.apiKey,
            useSSL: connectionSettings.useSSL,
            version: connectionSettings.version
        };

        dispatch(setReadarrClient({
            readarr: readarr
        }));

        return new Promise((resolve, reject) => {
            return { ok: false };
        });
    }
}


export function addReadarrCategory(category) {
    return (dispatch, getState) => {
        const state = getState();

        let categories = [...state.books.readarr.categories];
        categories.push(category);

        let readarr = {
            ...state.books.readarr,
            categories: categories
        };

        dispatch(setReadarrClient({
            readarr: readarr
        }));

        return new Promise((resolve, reject) => {
            return { ok: false };
        });
    }
};


export function removeReadarrCategory(categoryId) {
    return (dispatch, getState) => {
        const state = getState();

        let categories = [...state.books.readarr.categories];
        categories = categories.filter(x => x.id !== categoryId);

        let readarr = {
            ...state.books.readarr,
            categories: categories
        };

        dispatch(setReadarrClient({
            readarr: readarr
        }));

        return new Promise((resolve, reject) => {
            return { ok: false };
        });
    }
};


export function setReadarrCategory(categoryId, field, data) {
    return (dispatch, getState) => {
        const state = getState();

        let categories = [...state.books.readarr.categories];

        for (let index = 0; index < categories.length; index++) {
            if (categories[index].id === categoryId) {
                let category = { ...categories[index] };

                if (field === "name") {
                    category.name = data;
                } else if (field === "profileId") {
                    category.profileId = data;
                } else if (field === "metadataProfileId") {
                    category.metadataProfileId = data;
                } else if (field === "rootFolder") {
                    category.rootFolder = data;
                } else if (field === "tags") {
                    category.tags = state.books.readarr.tags.map(x => x.id).filter(x => data.includes(x));
                }

                categories[index] = category;
            }
        }

        let readarr = {
            ...state.books.readarr,
            categories: categories
        };
        console.log(readarr)

        dispatch(setReadarrClient({
            readarr: readarr
        }));

        return new Promise((resolve, reject) => {
            return { ok: false };
        });
    }
};


export function setReadarrCategories(categories) {
    return (dispatch, getState) => {
        const state = getState();

        let readarr = {
            ...state.books.readarr,
            categories: [...categories]
        };

        dispatch(setReadarrClient({
            readarr: readarr
        }));

        return new Promise((resolve, reject) => {
            return { ok: false };
        });
    }
};


export function loadReadarrRootPaths(forceReload) {
    return (dispatch, getState) => {
        const state = getState();

        var readarr = state.books.readarr;

        if ((!readarr.hasLoadedPaths && !readarr.isLoadingPaths) || forceReload) {
            dispatch(isLoadingReadarrPaths(true));

            return fetch("../api/book/readarr/rootpath", {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${state.user.token}`
                },
                body: JSON.stringify({
                    "Hostname": readarr.hostname,
                    'BaseUrl': readarr.baseUrl,
                    "Port": Number(readarr.port),
                    "ApiKey": readarr.apiKey,
                    "UseSSL": readarr.useSSL,
                    "Version": readarr.version,
                })
            })
                .then(data => {
                    if (data.status !== 200) {
                        throw new Error("Bad request.");
                    }

                    return data;
                })
                .then(data => data.json())
                .then(data => {
                    dispatch(setReadarrPaths(data));

                    return {
                        ok: true,
                        paths: data
                    }
                })
                .catch(err => {
                    dispatch(setReadarrPaths([]));
                    return { ok: false };
                })
        }
        else {
            return new Promise((resolve, reject) => {
                return { ok: false };
            });
        }
    };
};


export function loadReadarrProfiles(forceReload) {
    return (dispatch, getState) => {
        const state = getState();
        var readarr = state.books.readarr;

        if ((!readarr.hasLoadedProfiles && !readarr.isLoadingProfiles) || forceReload) {
            dispatch(isLoadingReadarrProfiles(true));

            return fetch("../api/book/readarr/profile", {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${state.user.token}`
                },
                body: JSON.stringify({
                    "Hostname": readarr.hostname,
                    'BaseUrl': readarr.baseUrl,
                    "Port": Number(readarr.port),
                    "ApiKey": readarr.apiKey,
                    "UseSSL": readarr.useSSL,
                    "Version": readarr.version,
                })
            })
                .then(data => {
                    if (data.status !== 200) {
                        throw new Error("Bad request.");
                    }

                    return data;
                })
                .then(data => data.json())
                .then(data => {
                    dispatch(setReadarrProfiles(data));

                    return {
                        ok: true,
                        profiles: data
                    }
                })
                .catch(err => {
                    dispatch(setReadarrProfiles([]));
                    return { ok: false };
                })
        } else {
            return new Promise((resolve, reject) => {
                return { ok: false };
            });
        }
    };
};


export function loadReadarrMetadataProfiles(forceReload) {
    return (dispatch, getState) => {
        const state = getState();
        var readarr = state.books.readarr;

        if ((!readarr.hasLoadedMetadataProfiles && !readarr.isLoadingMetadataProfiles) || forceReload) {
            dispatch(isLoadingReadarrMetadataProfiles(true));

            return fetch("../api/book/readarr/metadataprofile", {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${state.user.token}`
                },
                body: JSON.stringify({
                    "Hostname": readarr.hostname,
                    'BaseUrl': readarr.baseUrl,
                    "Port": Number(readarr.port),
                    "ApiKey": readarr.apiKey,
                    "UseSSL": readarr.useSSL,
                    "Version": readarr.version,
                })
            })
                .then(data => {
                    if (data.status !== 200) {
                        throw new Error("Bad request.");
                    }

                    return data;
                })
                .then(data => data.json())
                .then(data => {
                    dispatch(setReadarrMetadataProfiles(data));

                    return {
                        ok: true,
                        metadataProfiles: data
                    }
                })
                .catch(err => {
                    dispatch(setReadarrMetadataProfiles([]));
                    return { ok: false };
                })
        } else {
            return new Promise((resolve, reject) => {
                return { ok: false };
            });
        }
    };
};


export function loadReadarrTags(forceReload) {
    return (dispatch, getState) => {
        const state = getState();

        var readarr = state.books.readarr;

        if ((!readarr.hasLoadedTags && !readarr.isLoadingTags) || forceReload) {
            dispatch(isLoadingReadarrTags(true));

            return fetch("../api/book/readarr/tag", {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${state.user.token}`
                },
                body: JSON.stringify({
                    "Hostname": readarr.hostname,
                    'BaseUrl': readarr.baseUrl,
                    "Port": Number(readarr.port),
                    "ApiKey": readarr.apiKey,
                    "UseSSL": readarr.useSSL,
                    "Version": readarr.version,
                })
            })
                .then(data => {
                    if (data.status !== 200) {
                        throw new Error("Bad request.");
                    }

                    return data;
                })
                .then(data => data.json())
                .then(data => {
                    dispatch(setReadarrTags({ ok: true, data: data }));

                    return {
                        ok: true,
                        tags: data
                    }
                })
                .catch(err => {
                    dispatch(setReadarrTags({ ok: false, data: [] }));
                    return { ok: false };
                })
        }
        else {
            return new Promise((resolve, reject) => {
                return { ok: false };
            });
        }
    };
};


export function testReadarrSettings(settings) {
    return (dispatch, getState) => {
        const state = getState();

        return fetch("../api/book/readarr/test", {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Authorization": `Bearer ${state.user.token}`
            },
            body: JSON.stringify({
                "Hostname": settings.hostname,
                "BaseUrl": settings.baseUrl,
                "Port": Number(settings.port),
                "ApiKey": settings.apiKey,
                "UseSSL": settings.useSSL,
                "Version": settings.version
            })
        })
            .then(data => data.json())
            .then(data => {
                dispatch(loadReadarrProfiles(true));
                dispatch(loadReadarrMetadataProfiles(true));
                dispatch(loadReadarrRootPaths(true));
                dispatch(loadReadarrTags(true));

                if (data.ok)
                    return { ok: true };
                return { ok: false, error: data };
            });
    }
}


export function saveReadarrClient(saveModel) {
    return (dispatch, getState) => {
        const state = getState();

        return fetch("../api/book/readarr", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Authorization': `Bearer ${state.user.token}`
            },
            body: JSON.stringify({
                'Hostname': saveModel.readarr.hostname,
                'BaseUrl': saveModel.readarr.baseUrl,
                'Port': Number(saveModel.readarr.port),
                'ApiKey': saveModel.readarr.apiKey,
                'UseSSL': saveModel.readarr.useSSL,
                'Categories': state.books.readarr.categories,
                "Version": saveModel.readarr.version,
                'SearchNewRequests': saveModel.readarr.searchNewRequests,
                'MonitorNewRequests': saveModel.readarr.monitorNewRequests
            })
        })
            .then(data => data.json())
            .then(data => {
                if (data.ok) {
                    let newReadarr = {
                        ...state.books.readarr,
                        hostname: saveModel.readarr.hostname,
                        baseUrl: saveModel.readarr.baseUrl,
                        port: saveModel.readarr.port,
                        apiKey: saveModel.readarr.apiKey,
                        useSSL: saveModel.readarr.useSSL,
                        categories: state.books.readarr.categories,
                        searchNewRequests: saveModel.readarr.searchNewRequests,
                        monitorNewRequests: saveModel.readarr.monitorNewRequests,
                        version: saveModel.readarr.version
                    };

                    dispatch(setReadarrClient({
                        readarr: newReadarr
                    }));
                    return { ok: true };
                }

                return { ok: false, error: data };
            });
    }
}
