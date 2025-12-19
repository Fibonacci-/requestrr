
import { GET_SETTINGS, SET_DISABLED_CLIENT } from "../actions/BookClientsActions";


export default function BookClientsReducer(state = {}, action) {
    if (action.type === GET_SETTINGS) {
        return {
            ...state,
            client: action.payload.client,
            readarr: {
                hostname: action.payload.readarr.hostname,
                baseUrl: action.payload.readarr.baseUrl,
                port: action.payload.readarr.port,
                apiKey: action.payload.readarr.apiKey,
                useSSL: action.payload.readarr.useSSL,
                categories: action.payload.readarr.categories,
                searchNewRequests: action.payload.readarr.searchNewRequests,
                monitorNewRequests: action.payload.readarr.monitorNewRequests,
                version: action.payload.readarr.version,
                isLoadingPaths: false,
                hasLoadedPaths: false,
                arePathsValid: false,
                paths: [],
                isLoadingProfiles: false,
                hasLoadedProfiles: false,
                areProfilesValid: false,
                profiles: [],
                isLoadingMetadataProfiles: false,
                hasLoadedMetadataProfiles: false,
                areMetadataProfilesValid: false,
                metadataProfiles: [],
                isLoadingTags: false,
                hasLoadedTags: false,
                areTagsValid: false,
                tags: []
            },
            otherCategories: action.payload.otherCategories
        };
    } else if (action.type === SET_DISABLED_CLIENT) {
        return {
            ...state,
            client: "Disabled"
        };
    }

    return { ...state };
}
