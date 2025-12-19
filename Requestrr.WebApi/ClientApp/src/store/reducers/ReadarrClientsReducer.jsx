
import {
    READARR_SET_CLIENT,
    READARR_LOAD_PATHS,
    READARR_SET_PATHS,
    READARR_LOAD_PROFILES,
    READARR_SET_PROFILES,
    READARR_LOAD_METADATA_PROFILES,
    READARR_SET_METADATA_PROFILES,
    READARR_LOAD_TAGS,
    READARR_SET_TAGS
} from "../actions/ReadarrClientActions"

export default function ReadarrClientsReducer(state = {}, action) {
    var newState;
    var newReadarr;

    if (action.type === READARR_SET_CLIENT) {
        return {
            ...state,
            readarr: action.payload.readarr,
            client: "Readarr"
        };
    } else if (action.type === READARR_LOAD_PATHS) {
        newState = { ...state };
        newReadarr = { ...newState.readarr };

        newReadarr.isLoadingPaths = action.payload;

        newState.readarr = newReadarr;

        return newState;
    } else if (action.type === READARR_SET_PATHS) {
        newState = { ...state };
        newReadarr = { ...newState.readarr };

        newReadarr.isLoadingPaths = false;
        newReadarr.hasLoadedPaths = true;
        newReadarr.arePathsValid = action.payload.length > 0;
        newReadarr.paths = action.payload;

        newState.readarr = newReadarr;

        return newState;
    } else if (action.type === READARR_LOAD_PROFILES) {
        newState = { ...state };
        newReadarr = { ...newState.readarr };

        newReadarr.isLoadingProfiles = action.payload;

        newState.readarr = newReadarr;

        return newState;
    } else if (action.type === READARR_SET_PROFILES) {
        newState = { ...state };
        newReadarr = { ...newState.readarr };

        newReadarr.isLoadingProfiles = false;
        newReadarr.hasLoadedProfiles = true;
        newReadarr.areProfilesValid = action.payload.length > 0;
        newReadarr.profiles = action.payload;

        newState.readarr = newReadarr;

        return newState;
    } else if (action.type === READARR_LOAD_METADATA_PROFILES) {
        newState = { ...state };
        newReadarr = { ...newState.readarr };

        newReadarr.isLoadingMetadataProfiles = action.payload;

        newState.readarr = newReadarr;

        return newState;
    } else if (action.type === READARR_SET_METADATA_PROFILES) {
        newState = { ...state };
        newReadarr = { ...newState.readarr };

        newReadarr.isLoadingMetadataProfiles = false;
        newReadarr.hasLoadedMetadataProfiles = true;
        newReadarr.areMetadataProfilesValid = action.payload.length > 0;
        newReadarr.metadataProfiles = action.payload;

        newState.readarr = newReadarr;

        return newState;
    } else if (action.type === READARR_LOAD_TAGS) {
        newState = { ...state };
        newReadarr = { ...newState.readarr };

        newReadarr.isLoadingTags = action.payload;

        newState.readarr = newReadarr;

        return newState;
    } else if (action.type === READARR_SET_TAGS) {
        newState = { ...state };
        newReadarr = { ...newState.readarr };

        newReadarr.isLoadingTags = false;
        newReadarr.hasLoadedTags = true;
        newReadarr.areTagsValid = action.payload.ok;
        newReadarr.tags = action.payload.data;

        newState.readarr = newReadarr;

        return newState;
    }

    return { ...state };
}
