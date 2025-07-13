import {
  useDispatch as useReduxDispatch,
  useSelector as useReduxSelector,
} from "react-redux";
import {
  appSlice,
  deleteCloudDocument,
  getCloudDocument,
  loadLocalDocuments,
  createCloudDocument,
  updateCloudDocument,
  load,
  getLocalDocument,
  createLocalDocument,
  deleteLocalDocument,
  loadCloudDocuments,
  loadMyDocuments,
  toggleDocumentPublished,
  updateLocalDocument,
  updateUser,
  forkLocalDocument,
  forkCloudDocument,
  getLocalRevision,
  getCloudRevision,
  createLocalRevision,
  createCloudRevision,
  deleteLocalRevision,
  deleteCloudRevision,
  getLocalDocumentRevisions,
  alert,
  getLocalStorageUsage,
  getCloudStorageUsage,
} from "./app";
import { configureStore, ThunkAction, Action } from "@reduxjs/toolkit";

export const actions = {
  ...appSlice.actions,

  load,
  loadLocalDocuments,
  loadCloudDocuments,
  loadMyDocuments,

  getLocalDocument,
  createLocalDocument,
  updateLocalDocument,
  deleteLocalDocument,
  forkLocalDocument,

  getCloudDocument,
  createCloudDocument,
  updateCloudDocument,
  deleteCloudDocument,
  forkCloudDocument,
  toggleDocumentPublished,

  getLocalDocumentRevisions,
  getLocalRevision,
  getCloudRevision,
  createLocalRevision,
  createCloudRevision,
  deleteLocalRevision,
  deleteCloudRevision,

  updateUser,
  alert,

  getLocalStorageUsage,
  getCloudStorageUsage,
};

export const store = configureStore({ reducer: appSlice.reducer });

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>;

export const useDispatch: () => AppDispatch = useReduxDispatch;
export const useSelector: <T>(selector: (state: RootState) => T) => T =
  useReduxSelector;
