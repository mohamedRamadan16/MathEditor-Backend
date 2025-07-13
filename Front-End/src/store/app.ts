import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import NProgress from "nprogress";
import documentDB, { revisionDB } from "@/indexeddb";
import {
  AppState,
  Announcement,
  Alert,
  LocalDocument,
  User,
  PatchUserResponse,
  GetSessionResponse,
  DeleteRevisionResponse,
  GetRevisionResponse,
  ForkDocumentResponse,
  DocumentUpdateInput,
  EditorDocumentRevision,
  PostRevisionResponse,
  DocumentCreateInput,
  BackupDocument,
  CloudDocument,
  DocumentStorageUsage,
  GetDocumentStorageUsageResponse,
  GetDocumentThumbnailResponse,
} from "../types";
import {
  GetDocumentsResponse,
  PostDocumentsResponse,
  DeleteDocumentResponse,
  GetDocumentResponse,
  PatchDocumentResponse,
} from "@/types";
import { validate } from "uuid";
import { apiFetch } from "@/shared/api";
import MathEditorApiService from "@/services/api";
import {
  DocumentResponseDto,
  CreateRevisionDto,
  DocumentCreateDto,
  DocumentUpdateDto,
} from "@/types/api";
import { SerializedEditorState } from "lexical";

const initialState: AppState = {
  documents: [],
  ui: {
    announcements: [],
    alerts: [],
    initialized: false,
    drawer: false,
    page: 1,
    diff: {
      open: false,
    },
  },
};

export const load = createAsyncThunk("app/load", async (_, thunkAPI) => {
  await Promise.allSettled([
    thunkAPI.dispatch(loadSession()),
    thunkAPI.dispatch(loadLocalDocuments()),
    thunkAPI.dispatch(loadCloudDocuments()),
  ]);
});

export const loadSession = createAsyncThunk(
  "app/loadSession",
  async (_, thunkAPI) => {
    try {
      const apiService = MathEditorApiService.getInstance();
      const token = apiService.getToken();

      if (!token) {
        return thunkAPI.fulfillWithValue(undefined);
      }

      const response = await apiService.getCurrentUser();

      if (!response.success || !response.data) {
        return thunkAPI.fulfillWithValue(undefined);
      }

      const user: User = {
        id: response.data.id,
        handle: response.data.handle,
        name: response.data.name,
        email: response.data.email,
        image: response.data.image || null,
      };

      return thunkAPI.fulfillWithValue(user);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const loadLocalDocuments = createAsyncThunk(
  "app/loadLocalDocuments",
  async (_, thunkAPI) => {
    try {
      const documents = await documentDB.getAll();
      const revisions = await revisionDB.getAll();
      const localDocuments: LocalDocument[] = await Promise.all(
        documents.map(async (document) => {
          const { data, ...rest } = document;
          const backupDocument: BackupDocument = {
            ...document,
            revisions: revisions.filter(
              (revision) => revision.documentId === document.id
            ),
          };
          const localRevisions = backupDocument.revisions.map(
            ({ data, ...rest }) => rest
          );
          const localDocument: LocalDocument = {
            ...rest,
            revisions: localRevisions,
          };
          return localDocument;
        })
      );
      return thunkAPI.fulfillWithValue(localDocuments);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const loadCloudDocuments = createAsyncThunk(
  "app/loadCloudDocuments",
  async (payloadCreator: CloudDocument[] | undefined, thunkAPI) => {
    try {
      NProgress.start();
      if (payloadCreator) return thunkAPI.fulfillWithValue(payloadCreator);

      const apiService = MathEditorApiService.getInstance();
      const response = await apiService.getAllDocuments();

      if (!response.success || !response.data) {
        return thunkAPI.fulfillWithValue([]);
      }

      // Convert API response to CloudDocument format
      const cloudDocuments: CloudDocument[] = response.data.items.map(
        (doc: DocumentResponseDto) => ({
          id: doc.id,
          handle: doc.handle,
          name: doc.name,
          createdAt: doc.createdAt,
          updatedAt: doc.updatedAt,
          head: doc.head,
          baseId: doc.baseId,
          published: doc.published,
          collab: doc.collab,
          private: doc.private,
          author: {
            id: doc.author.id,
            handle: doc.author.handle,
            name: doc.author.name,
            email: doc.author.email,
            image: doc.author.image || null,
          },
          coauthors: doc.coauthors.map((coauthor) => ({
            id: coauthor.id,
            handle: coauthor.handle,
            name: coauthor.name,
            email: coauthor.email,
            image: coauthor.image || null,
          })),
          revisions: doc.revisions.map((revision) => ({
            id: revision.id,
            documentId: revision.documentId,
            createdAt: revision.createdAt,
            author: {
              id: revision.author.id,
              handle: revision.author.handle,
              name: revision.author.name,
              email: revision.author.email,
              image: revision.author.image || null,
            },
          })),
        })
      );

      return thunkAPI.fulfillWithValue(cloudDocuments);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    } finally {
      NProgress.done();
    }
  }
);

export const loadMyDocuments = createAsyncThunk(
  "app/loadMyDocuments",
  async (_, thunkAPI) => {
    try {
      NProgress.start();
      const apiService = MathEditorApiService.getInstance();
      const response = await apiService.getMyDocuments();

      if (!response.success || !response.data) {
        return thunkAPI.fulfillWithValue([]);
      }

      // Convert API response to CloudDocument format
      const cloudDocuments: CloudDocument[] = response.data.items.map(
        (doc: DocumentResponseDto) => ({
          id: doc.id,
          handle: doc.handle,
          name: doc.name,
          createdAt: doc.createdAt,
          updatedAt: doc.updatedAt,
          head: doc.head,
          baseId: doc.baseId,
          published: doc.published,
          collab: doc.collab,
          private: doc.private,
          author: {
            id: doc.author.id,
            handle: doc.author.handle,
            name: doc.author.name,
            email: doc.author.email,
            image: doc.author.image || null,
          },
          coauthors: doc.coauthors.map((coauthor) => ({
            id: coauthor.id,
            handle: coauthor.handle,
            name: coauthor.name,
            email: coauthor.email,
            image: coauthor.image || null,
          })),
          revisions: doc.revisions.map((revision) => ({
            id: revision.id,
            documentId: revision.documentId,
            createdAt: revision.createdAt,
            author: {
              id: revision.author.id,
              handle: revision.author.handle,
              name: revision.author.name,
              email: revision.author.email,
              image: revision.author.image || null,
            },
          })),
        })
      );

      return thunkAPI.fulfillWithValue(cloudDocuments);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    } finally {
      NProgress.done();
    }
  }
);

export const getLocalStorageUsage = createAsyncThunk(
  "app/getLocalStorageUsage",
  async (_, thunkAPI) => {
    try {
      const documents = await documentDB.getAll();
      const revisions = await revisionDB.getAll();
      const localStorageUsage: DocumentStorageUsage[] = [];
      documents
        .sort((a, b) => {
          const first = a.updatedAt;
          const second = b.updatedAt;
          if (!first && !second) return 0;
          if (!first) return 1;
          if (!second) return -1;
          return new Date(second).getTime() - new Date(first).getTime();
        })
        .map((document) => {
          const backupDocument: BackupDocument = {
            ...document,
            revisions: revisions.filter(
              (revision) => revision.documentId === document.id
            ),
          };
          const backupDocumentSize = new Blob([JSON.stringify(backupDocument)])
            .size;
          localStorageUsage.push({
            id: document.id,
            name: document.name,
            size: backupDocumentSize,
          });
        });
      return thunkAPI.fulfillWithValue(localStorageUsage);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const getCloudStorageUsage = createAsyncThunk(
  "app/getCloudStorageUsage",
  async (_, thunkAPI) => {
    try {
      const response = await apiFetch("/usage");
      const { data, error } =
        (await response.json()) as GetDocumentStorageUsageResponse;
      if (error) return thunkAPI.rejectWithValue(error);
      if (!data) return thunkAPI.fulfillWithValue([]);
      return thunkAPI.fulfillWithValue(data);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

// Removed thumbnail API call as no backend endpoint exists

export const getLocalDocument = createAsyncThunk(
  "app/getLocalDocument",
  async (id: string, thunkAPI: any) => {
    try {
      const isValidId = validate(id);
      const document = isValidId
        ? await documentDB.getByID(id)
        : await documentDB.getOneByKey("handle", id);
      if (!document)
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "document not found",
        });
      return thunkAPI.fulfillWithValue(document);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const getLocalRevision = createAsyncThunk(
  "app/getLocalRevision",
  async (id: string, thunkAPI: any) => {
    try {
      const revision = await revisionDB.getByID(id);
      if (!revision)
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "revision not found",
        });
      return thunkAPI.fulfillWithValue(revision);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const getLocalDocumentRevisions = createAsyncThunk(
  "app/getLocalDocumentRevisions",
  async (id: string, thunkAPI: any) => {
    try {
      const revisions = await revisionDB.getManyByKey("documentId", id);
      return thunkAPI.fulfillWithValue(revisions);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const getCloudDocument = createAsyncThunk(
  "app/getCloudDocument",
  async (id: string, thunkAPI: any) => {
    try {
      const apiService = MathEditorApiService.getInstance();

      // Use correct endpoint for handle vs GUID
      const isGuid = validate(id);
      let apiResponse;

      if (isGuid) {
        apiResponse = await apiService.getDocument(id);
      } else {
        apiResponse = await apiService.getDocumentByHandle(id);
      }

      // .NET returns { data, success, message }
      if (!apiResponse.success || !apiResponse.data) {
        return thunkAPI.rejectWithValue({
          title: "Not found",
          subtitle: apiResponse.message || "Document not found",
        });
      }

      // Map .NET DocumentResponseDto to expected frontend structure if needed
      const doc = apiResponse.data;
      // Fetch the latest revision using the document's head property
      let latestRevision = undefined;
      if (doc.head) {
        try {
          const revResponse = await apiService.getRevision(doc.head);

          if (revResponse.success && revResponse.data) {
            // Check if the revision data is valid JSON (not just "string")
            if (typeof revResponse.data.data === "string") {
              try {
                const parsedData = JSON.parse(revResponse.data.data);
                // Validate it's a proper Lexical state
                if (
                  parsedData &&
                  parsedData.root &&
                  Array.isArray(parsedData.root.children)
                ) {
                  // Convert format fields from strings to numbers for Lexical compatibility
                  const convertNode = (node: any): any => {
                    if (!node) return node;

                    const converted: any = {
                      ...node,
                      children: node.children
                        ? node.children.map(convertNode)
                        : [],
                    };

                    // Convert format from string to number if it's a text node
                    if (node.type === "text" && node.format !== undefined) {
                      converted.format =
                        typeof node.format === "string"
                          ? parseInt(node.format, 10) || 0
                          : node.format;
                    }

                    // Convert detail from string to number if it exists
                    if (node.detail !== undefined) {
                      converted.detail =
                        typeof node.detail === "string"
                          ? parseInt(node.detail, 10) || 0
                          : node.detail;
                    }

                    return converted;
                  };

                  const convertedData = {
                    root: {
                      ...parsedData.root,
                      children: parsedData.root.children.map(convertNode),
                      format:
                        typeof parsedData.root.format === "string"
                          ? parseInt(parsedData.root.format, 10) || 0
                          : parsedData.root.format || 0,
                    },
                  };

                  latestRevision = {
                    ...revResponse.data,
                    data: convertedData,
                  };
                } else {
                  console.warn(
                    "[Lexical Debug] Invalid Lexical state structure, using default."
                  );
                  latestRevision = {
                    ...revResponse.data,
                    data: {
                      root: {
                        children: [
                          {
                            children: [],
                            direction: null,
                            format: "",
                            indent: 0,
                            type: "paragraph",
                            version: 1,
                          },
                        ],
                        direction: "ltr",
                        format: "",
                        indent: 0,
                        type: "root",
                        version: 1,
                      },
                    },
                  };
                }
              } catch (parseErr) {
                console.warn(
                  "[Lexical Debug] Failed to parse revision data as JSON, using default state."
                );
                latestRevision = {
                  ...revResponse.data,
                  data: {
                    root: {
                      children: [
                        {
                          children: [],
                          direction: null,
                          format: "",
                          indent: 0,
                          type: "paragraph",
                          version: 1,
                        },
                      ],
                      direction: "ltr",
                      format: "",
                      indent: 0,
                      type: "root",
                      version: 1,
                    },
                  },
                };
              }
            } else if (typeof revResponse.data.data === "object") {
              // Data is already an object, use it directly
              latestRevision = revResponse.data;
            }
          }
        } catch (revErr) {
          console.error("[Lexical Debug] Error fetching revision:", revErr);
        }
      }

      // Transform the document to include the current revision data
      // The editor expects the document to have a 'data' property with the current content
      const transformedDocument = {
        ...doc,
        data: latestRevision?.data || {
          root: {
            children: [
              {
                children: [],
                direction: null,
                format: "",
                indent: 0,
                type: "paragraph",
                version: 1,
              },
            ],
            direction: "ltr",
            format: "",
            indent: 0,
            type: "root",
            version: 1,
          },
        },
        latestRevision,
      };

      return thunkAPI.fulfillWithValue(transformedDocument);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const getCloudRevision = createAsyncThunk(
  "app/getCloudRevision",
  async (id: string, thunkAPI: any) => {
    try {
      const response = await apiFetch(`/revisions/${id}`);
      const { data, error } = (await response.json()) as GetRevisionResponse;
      if (error) return thunkAPI.rejectWithValue(error);
      if (!data)
        return thunkAPI.rejectWithValue({
          title: "Not found",
          subtitle: "Revision not found",
        });
      return thunkAPI.fulfillWithValue(data);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const forkLocalDocument = createAsyncThunk(
  "app/forkLocalDocument",
  async (
    payloadCreator: { id: string; revisionId?: string | null },
    thunkAPI: any
  ) => {
    try {
      const { id, revisionId } = payloadCreator;
      const isValidId = validate(id);
      const document = isValidId
        ? await documentDB.getByID(id)
        : await documentDB.getOneByKey("handle", id);
      if (!document)
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "document not found",
        });
      if (!revisionId || revisionId === document.head)
        return thunkAPI.fulfillWithValue(document);
      const revision = await revisionDB.getByID(revisionId);
      if (!revision)
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "revision not found",
        });
      document.head = revision.id;
      document.updatedAt = revision.createdAt;
      document.data = revision.data;
      return thunkAPI.fulfillWithValue(document);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const forkCloudDocument = createAsyncThunk(
  "app/forkCloudDocument",
  async (
    payloadCreator: { id: string; revisionId?: string | null },
    thunkAPI: any
  ) => {
    try {
      const apiService = MathEditorApiService.getInstance();

      // Check if id is a GUID or handle, similar to getCloudDocument
      const isGuid = validate(payloadCreator.id);
      let documentId: string;

      if (isGuid) {
        documentId = payloadCreator.id;
      } else {
        // If it's a handle, first get the document to get its GUID
        const docResponse = await apiService.getDocumentByHandle(
          payloadCreator.id
        );
        if (!docResponse.success || !docResponse.data) {
          return thunkAPI.rejectWithValue({
            title: "Document not found",
            subtitle: `Could not find document with handle: ${payloadCreator.id}`,
          });
        }
        documentId = docResponse.data.id;
      }

      // Now fork the document using the GUID
      const forkResponse = await apiService.forkDocument(documentId);

      if (!forkResponse.success || !forkResponse.data) {
        return thunkAPI.rejectWithValue({
          title: "Fork failed",
          subtitle: forkResponse.message || "Could not fork document",
        });
      }

      const forkedDocument = forkResponse.data;

      // Now fetch the document data using the same logic as getCloudDocument
      let editorData = undefined;
      if (forkedDocument.head) {
        try {
          const revResponse = await apiService.getRevision(forkedDocument.head);

          if (revResponse.success && revResponse.data) {
            // Check if the revision data is valid JSON (not just "string")
            if (typeof revResponse.data.data === "string") {
              try {
                const parsedData = JSON.parse(revResponse.data.data);
                // Validate it's a proper Lexical state
                if (
                  parsedData &&
                  parsedData.root &&
                  Array.isArray(parsedData.root.children)
                ) {
                  // Convert format fields from strings to numbers for Lexical compatibility
                  const convertNode = (node: any): any => {
                    if (!node) return node;

                    const converted: any = {
                      ...node,
                      children: node.children
                        ? node.children.map(convertNode)
                        : [],
                    };

                    // Convert format from string to number if it's a text node
                    if (node.type === "text" && node.format !== undefined) {
                      converted.format =
                        typeof node.format === "string"
                          ? parseInt(node.format, 10) || 0
                          : node.format;
                    }

                    // Convert detail from string to number if it exists
                    if (node.detail !== undefined) {
                      converted.detail =
                        typeof node.detail === "string"
                          ? parseInt(node.detail, 10) || 0
                          : node.detail;
                    }

                    // Convert indent from string to number if it exists
                    if (node.indent !== undefined) {
                      converted.indent =
                        typeof node.indent === "string"
                          ? parseInt(node.indent, 10) || 0
                          : node.indent;
                    }

                    return converted;
                  };

                  editorData = {
                    ...parsedData,
                    root: convertNode(parsedData.root),
                  };
                } else {
                  console.warn("Invalid Lexical state format:", parsedData);
                  editorData = {
                    root: {
                      children: [],
                      direction: "ltr",
                      format: "",
                      indent: 0,
                      type: "root",
                      version: 1,
                    },
                  };
                }
              } catch (parseError) {
                console.error("Failed to parse revision data:", parseError);
                editorData = {
                  root: {
                    children: [],
                    direction: "ltr",
                    format: "",
                    indent: 0,
                    type: "root",
                    version: 1,
                  },
                };
              }
            } else {
              // Data is already parsed
              editorData = revResponse.data.data;
            }
          }
        } catch (revError) {
          console.error("Failed to fetch revision data:", revError);
        }
      }

      // If we couldn't get the data, use default
      if (!editorData) {
        editorData = {
          root: {
            children: [],
            direction: "ltr",
            format: "",
            indent: 0,
            type: "root",
            version: 1,
          },
        };
      }

      // Return format expected by NewDocument component
      const result = {
        id: forkedDocument.id,
        cloud: forkedDocument,
        data: editorData,
      };

      return thunkAPI.fulfillWithValue(result);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const createLocalDocument = createAsyncThunk(
  "app/createLocalDocument",
  async (payloadCreator: DocumentCreateInput, thunkAPI: any) => {
    try {
      const {
        coauthors,
        published,
        collab,
        private: isPrivate,
        revisions,
        ...document
      } = payloadCreator;
      const id = await documentDB.add(document);
      if (!id)
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "failed to create document",
        });
      const { data, ...rest } = document;
      if (revisions) await revisionDB.addMany(revisions);
      const localDocumentRevisions = (revisions ?? []).map(
        ({ data, ...rest }) => rest
      );
      const localDocument: LocalDocument = {
        ...rest,
        revisions: localDocumentRevisions,
      };
      return thunkAPI.fulfillWithValue(localDocument);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const createLocalRevision = createAsyncThunk(
  "app/createLocalRevision",
  async (revision: EditorDocumentRevision, thunkAPI: any) => {
    try {
      const id = await revisionDB.add(revision);
      if (!id)
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "failed to create revision",
        });
      const { data, ...rest } = revision;
      return thunkAPI.fulfillWithValue(rest);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const createCloudDocument = createAsyncThunk(
  "app/createCloudDocument",
  async (payloadCreator: DocumentCreateInput, thunkAPI: any) => {
    try {
      const apiService = MathEditorApiService.getInstance();
      const state = thunkAPI.getState();

      // Check if this local document already has a cloud version
      const existingUserDocument = state.documents.find(
        (doc: any) => doc.id === payloadCreator.id && doc.cloud
      );

      if (existingUserDocument && existingUserDocument.cloud) {
        // Document already exists in cloud, create a new revision instead
        const newRevision: EditorDocumentRevision = {
          id: "", // Will be set by the backend
          documentId: existingUserDocument.cloud.id,
          data: payloadCreator.data,
          createdAt: new Date().toISOString(),
        };

        // Create the revision and return the updated cloud document
        const revisionResponse = await thunkAPI.dispatch(
          createCloudRevision(newRevision)
        );
        if (revisionResponse.type === createCloudRevision.fulfilled.type) {
          // Return the existing cloud document (it will be updated by the reducer)
          return thunkAPI.fulfillWithValue(existingUserDocument.cloud);
        } else {
          return thunkAPI.rejectWithValue({
            title: "Failed to save revision",
            subtitle: "Could not save changes to cloud document",
          });
        }
      }

      // Generate a unique handle if none provided (fallback safety)
      const generateHandle = (name: string) => {
        const baseHandle = name
          .toLowerCase()
          .replace(/[^a-z0-9\s-]/g, "") // Remove special characters
          .replace(/\s+/g, "-") // Replace spaces with hyphens
          .replace(/-+/g, "-") // Replace multiple hyphens with single hyphen
          .replace(/^-|-$/g, "") // Remove leading/trailing hyphens
          .substring(0, 20); // Limit length to leave room for uniqueness suffix

        // Add timestamp and random component to ensure uniqueness
        const timestamp = Date.now().toString(36);
        const random = Math.random().toString(36).substring(2, 8);
        return `${baseHandle || "doc"}-${timestamp}-${random}`;
      };

      // Always generate a new unique handle for cloud documents to prevent duplicates
      const originalHandle = payloadCreator.handle;
      const uniqueHandle =
        originalHandle && originalHandle.length > 0
          ? `${originalHandle}-${Date.now().toString(36)}-${Math.random()
              .toString(36)
              .substring(2, 5)}`
          : generateHandle(payloadCreator.name);

      // Convert frontend format to backend DTO format
      const now = new Date().toISOString();
      const documentCreateDto: DocumentCreateDto = {
        handle: uniqueHandle,
        name: payloadCreator.name,
        createdAt: now,
        updatedAt: now,
        published: payloadCreator.published || false,
        private: payloadCreator.private || false,
        collab: payloadCreator.collab || false,
        coauthors: payloadCreator.coauthors || [],
        initialRevision: {
          data: MathEditorApiService.convertSerializedEditorStateToLexicalState(
            payloadCreator.data || {
              root: {
                children: [],
                direction: "ltr",
                format: "",
                indent: 0,
                type: "root",
                version: 1,
              },
            }
          ),
        },
      };

      const response = await apiService.createDocument(documentCreateDto);

      if (!response.success || !response.data) {
        return thunkAPI.rejectWithValue({
          title: "Not created",
          subtitle: response.message || "Document not created",
        });
      }

      return thunkAPI.fulfillWithValue(response.data);
    } catch (error: any) {
      console.error("Error creating cloud document:", error);

      // Check if this is a duplicate handle error
      if (error.message && error.message.includes("already exists")) {
        return thunkAPI.rejectWithValue({
          title: "Handle already exists",
          subtitle:
            "A document with this handle already exists. Please choose a different handle.",
        });
      }

      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const createCloudRevision = createAsyncThunk(
  "app/createCloudRevision",
  async (revision: EditorDocumentRevision, thunkAPI: any) => {
    try {
      NProgress.start();
      const apiService = MathEditorApiService.getInstance();

      // Convert frontend format to backend DTO format
      const createRevisionDto: CreateRevisionDto = {
        documentId: revision.documentId,
        data: MathEditorApiService.convertSerializedEditorStateToLexicalState(
          revision.data
        ),
      };

      const response = await apiService.createRevision(createRevisionDto);

      if (!response.success || !response.data) {
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "failed to create revision",
        });
      }

      return thunkAPI.fulfillWithValue(response.data);
    } catch (error: any) {
      console.error("Error creating cloud revision:", error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    } finally {
      NProgress.done();
    }
  }
);

export const updateLocalDocument = createAsyncThunk(
  "app/updateLocalDocument",
  async (
    payloadCreator: { id: string; partial: DocumentUpdateInput },
    thunkAPI: any
  ) => {
    try {
      const { id, partial } = payloadCreator;
      const {
        coauthors,
        published,
        collab,
        private: isPrivate,
        revisions,
        ...document
      } = partial;
      const result = await documentDB.patch(id, document);
      if (!result)
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "failed to update document",
        });
      const payload: { id: string; partial: Partial<LocalDocument> } = {
        id,
        partial: { ...document },
      };
      if (revisions) {
        await revisionDB.addMany(revisions);
        const localDocumentRevisions = (revisions ?? []).map(
          ({ data, ...rest }) => rest
        );
        payload.partial.revisions = localDocumentRevisions;
      }

      return thunkAPI.fulfillWithValue(payload);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const updateCloudDocument = createAsyncThunk(
  "app/updateCloudDocument",
  async (
    payloadCreator: { id: string; partial: DocumentUpdateInput },
    thunkAPI: any
  ) => {
    try {
      const apiService = MathEditorApiService.getInstance();

      // Convert frontend format to backend DTO format
      const documentUpdateDto: DocumentUpdateDto = {
        id: payloadCreator.id,
        handle: payloadCreator.partial.handle || undefined,
        name: payloadCreator.partial.name || undefined,
        published:
          payloadCreator.partial.published !== undefined
            ? payloadCreator.partial.published
            : undefined,
        private:
          payloadCreator.partial.private !== undefined
            ? payloadCreator.partial.private
            : undefined,
        collab:
          payloadCreator.partial.collab !== undefined
            ? payloadCreator.partial.collab
            : undefined,
        // coauthors are handled separately via dedicated endpoints
      };

      const response = await apiService.updateDocument(documentUpdateDto);

      if (!response.success || !response.data) {
        return thunkAPI.rejectWithValue({
          title: "Not updated",
          subtitle: "Document not updated",
        });
      }

      return thunkAPI.fulfillWithValue(response.data);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const toggleDocumentPublished = createAsyncThunk(
  "app/toggleDocumentPublished",
  async (payloadCreator: { id: string; published: boolean }, thunkAPI: any) => {
    try {
      const apiService = MathEditorApiService.getInstance();
      const response = await apiService.toggleDocumentPublished(
        payloadCreator.id,
        payloadCreator.published
      );

      if (!response.success || !response.data) {
        return thunkAPI.rejectWithValue({
          title: "Not updated",
          subtitle: "Document publish status not updated",
        });
      }

      return thunkAPI.fulfillWithValue(response.data);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const deleteLocalDocument = createAsyncThunk(
  "app/deleteLocalDocument",
  async (id: string, thunkAPI: any) => {
    try {
      await documentDB.deleteByID(id);
      await revisionDB.deleteManyByKey("documentId", id);
      return thunkAPI.fulfillWithValue(id);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const deleteLocalRevision = createAsyncThunk(
  "app/deleteLocalRevision",
  async (payloadCreator: { id: string; documentId: string }, thunkAPI: any) => {
    try {
      await revisionDB.deleteByID(payloadCreator.id);
      return thunkAPI.fulfillWithValue(payloadCreator);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const deleteCloudDocument = createAsyncThunk(
  "app/deleteCloudDocument",
  async (id: string, thunkAPI: any) => {
    try {
      NProgress.start();
      const apiService = MathEditorApiService.getInstance();

      const response = await apiService.deleteDocument(id);

      if (!response.success) {
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "failed to delete document",
        });
      }

      return thunkAPI.fulfillWithValue({ id });
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    } finally {
      NProgress.done();
    }
  }
);

export const deleteCloudRevision = createAsyncThunk(
  "app/deleteCloudRevision",
  async (payloadCreator: { id: string; documentId: string }, thunkAPI: any) => {
    try {
      NProgress.start();
      const apiService = MathEditorApiService.getInstance();

      const response = await apiService.deleteRevision(payloadCreator.id);

      if (!response.success) {
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "failed to delete revision",
        });
      }

      return thunkAPI.fulfillWithValue({
        id: payloadCreator.id,
        documentId: payloadCreator.documentId,
      });
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    } finally {
      NProgress.done();
    }
  }
);

export const updateUser = createAsyncThunk(
  "app/updateUser",
  async (
    payloadCreator: { id: string; partial: Partial<User> },
    thunkAPI: any
  ) => {
    try {
      NProgress.start();
      const { id, partial } = payloadCreator;
      const response = await apiFetch(`/users/${id}`, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(partial),
      });
      const { data, error } = (await response.json()) as PatchUserResponse;
      if (error) return thunkAPI.rejectWithValue(error);
      if (!data)
        return thunkAPI.rejectWithValue({
          title: "Something went wrong",
          subtitle: "failed to update user",
        });
      const payload: User = data;
      return thunkAPI.fulfillWithValue(payload);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    } finally {
      NProgress.done();
    }
  }
);

export const alert = createAsyncThunk(
  "app/alert",
  async (payloadCreator: Alert, thunkAPI: any) => {
    try {
      const id = await new Promise((resolve) => {
        const handler = (event: MouseEvent): any => {
          const target = event.target as HTMLElement;
          const button = target.closest("button");
          const paper = target.closest(".MuiDialog-paper");
          if (paper && !button)
            return document.addEventListener("click", handler, { once: true });
          resolve(button?.id ?? null);
        };
        setTimeout(() => {
          document.addEventListener("click", handler, { once: true });
        }, 0);
      });
      return thunkAPI.fulfillWithValue(id);
    } catch (error: any) {
      console.error(error);
      return thunkAPI.rejectWithValue({
        title: "Something went wrong",
        subtitle: error.message,
      });
    }
  }
);

export const appSlice = createSlice({
  name: "app",
  initialState,
  reducers: {
    setUser(state, action: PayloadAction<AppState["user"]>) {
      state.user = action.payload;
    },
    announce: (state, action: PayloadAction<Announcement>) => {
      state.ui.announcements.push(action.payload);
    },
    clearAnnouncement: (state: any) => {
      state.ui.announcements.shift();
    },
    clearAlert: (state: any) => {
      state.ui.alerts.shift();
    },
    toggleDrawer: (state: any, action: PayloadAction<boolean | undefined>) => {
      if (action.payload !== undefined) state.ui.drawer = action.payload;
      else state.ui.drawer = !state.ui.drawer;
    },
    setPage: (state: any, action: PayloadAction<number>) => {
      state.ui.page = action.payload;
    },
    setDiff: (
      state: any,
      action: PayloadAction<Partial<AppState["ui"]["diff"]>>
    ) => {
      state.ui.diff = { ...state.ui.diff, ...action.payload };
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(load.fulfilled, (state, action) => {
        state.documents = state.documents.sort((a, b) => {
          const first = a.local?.updatedAt || a.cloud?.updatedAt;
          const second = b.local?.updatedAt || b.cloud?.updatedAt;
          if (!first && !second) return 0;
          if (!first) return 1;
          if (!second) return -1;
          return new Date(second).getTime() - new Date(first).getTime();
        });
        state.ui.initialized = true;
      })
      .addCase(loadSession.fulfilled, (state, action) => {
        const user = action.payload;
        state.user = user;
      })
      .addCase(loadLocalDocuments.fulfilled, (state, action) => {
        const documents = action.payload;
        documents.forEach((document) => {
          const userDocument = state.documents.find(
            (doc) => doc.id === document.id
          );
          if (!userDocument)
            state.documents.push({ id: document.id, local: document });
          else userDocument.local = document;
        });
      })
      .addCase(loadCloudDocuments.fulfilled, (state, action) => {
        const documents = action.payload;
        documents.forEach((document) => {
          const userDocument = state.documents.find(
            (doc) => doc.id === document.id
          );
          if (!userDocument)
            state.documents.push({ id: document.id, cloud: document });
          else userDocument.cloud = document;
        });
      })
      .addCase(loadMyDocuments.fulfilled, (state, action) => {
        const documents = action.payload;
        documents.forEach((document) => {
          const userDocument = state.documents.find(
            (doc) => doc.id === document.id
          );
          if (!userDocument)
            state.documents.push({ id: document.id, cloud: document });
          else userDocument.cloud = document;
        });
      })
      .addCase(getCloudDocument.fulfilled, (state, action) => {
        const cloudDocument = action.payload;
        const userDocument = state.documents.find(
          (doc) => doc.id === cloudDocument.id
        );
        if (!userDocument)
          state.documents.unshift({
            id: cloudDocument.id,
            cloud: cloudDocument,
          });
        else userDocument.cloud = cloudDocument;
      })
      .addCase(getCloudRevision.rejected, (state, action) => {
        const message = action.payload as { title: string; subtitle: string };
        state.ui.announcements.push({ message });
      })
      .addCase(forkCloudDocument.rejected, (state, action) => {
        const message = action.payload as { title: string; subtitle: string };
        state.ui.announcements.push({ message });
      })
      .addCase(createLocalDocument.fulfilled, (state, action) => {
        const document = action.payload;
        const userDocument = state.documents.find(
          (doc) => doc.id === document.id
        );
        if (!userDocument)
          state.documents.unshift({ id: document.id, local: document });
        else userDocument.local = document;
      })
      .addCase(createLocalRevision.fulfilled, (state, action) => {
        const revision = action.payload;
        const userDocument = state.documents.find(
          (doc) => doc.id === revision.documentId
        );
        if (!userDocument) return;
        const localDocument = userDocument.local;
        if (!localDocument) return;
        localDocument.revisions.unshift(revision);
      })
      .addCase(createCloudDocument.fulfilled, (state, action) => {
        const document = action.payload;
        const userDocument = state.documents.find(
          (doc) => doc.id === document.id
        );
        if (!userDocument)
          state.documents.unshift({ id: document.id, cloud: document });
        else userDocument.cloud = document;
      })
      .addCase(createCloudDocument.rejected, (state, action) => {
        const message = action.payload as { title: string; subtitle: string };
        state.ui.announcements.push({ message });
      })
      .addCase(createCloudRevision.fulfilled, (state, action) => {
        const revision = action.payload;
        // Find document by cloud document ID (revision.documentId refers to cloud document)
        const document = state.documents.find(
          (doc) => doc.cloud && doc.cloud.id === revision.documentId
        );
        if (!document?.cloud) return;
        document.cloud.revisions.unshift(revision);
        // Update the head to point to the new revision
        document.cloud.head = revision.id;
        document.cloud.updatedAt = revision.createdAt;
      })
      .addCase(createCloudRevision.rejected, (state, action) => {
        const message = action.payload as { title: string; subtitle: string };
        state.ui.announcements.push({ message });
      })
      .addCase(updateLocalDocument.fulfilled, (state, action) => {
        const { id, partial } = action.payload;
        const userDocument = state.documents.find((doc) => doc.id === id);
        if (!userDocument) return;
        const localDocument = userDocument.local;
        if (!localDocument) return;
        Object.assign(localDocument, partial);
      })
      .addCase(updateCloudDocument.fulfilled, (state, action) => {
        const document = action.payload;
        const userDocument = state.documents.find(
          (doc) => doc.id === document.id
        );
        if (!userDocument)
          state.documents.unshift({ id: document.id, cloud: document });
        else userDocument.cloud = document;
      })
      .addCase(updateCloudDocument.rejected, (state, action) => {
        const message = action.payload as { title: string; subtitle: string };
        state.ui.announcements.push({ message });
      })
      .addCase(toggleDocumentPublished.fulfilled, (state, action) => {
        const document = action.payload;
        const userDocument = state.documents.find(
          (doc) => doc.id === document.id
        );
        if (!userDocument)
          state.documents.unshift({ id: document.id, cloud: document });
        else userDocument.cloud = document;
      })
      .addCase(toggleDocumentPublished.rejected, (state, action) => {
        const message = action.payload as { title: string; subtitle: string };
        state.ui.announcements.push({ message });
      })
      .addCase(deleteLocalDocument.fulfilled, (state, action) => {
        const id = action.payload;
        const userDocument = state.documents.find((doc) => doc.id === id);
        if (!userDocument) return;
        if (!userDocument.cloud)
          state.documents.splice(state.documents.indexOf(userDocument), 1);
        else delete userDocument.local;
      })
      .addCase(deleteLocalRevision.fulfilled, (state, action) => {
        const { id, documentId } = action.payload;
        const userDocument = state.documents.find(
          (doc) => doc.id === documentId
        );
        if (!userDocument) return;
        const localDocument = userDocument.local;
        if (!localDocument) return;
        const revision = localDocument.revisions.find(
          (revision) => revision.id === id
        );
        if (!revision) return;
        localDocument.revisions = localDocument.revisions.filter(
          (revision) => revision.id !== id
        );
      })
      .addCase(deleteCloudDocument.fulfilled, (state, action) => {
        const id = action.payload;
        const userDocument = state.documents.find((doc) => doc.id === id);
        if (!userDocument) return;
        const index = state.documents.indexOf(userDocument);
        if (!userDocument.local) state.documents.splice(index, 1);
        else delete userDocument.cloud;
      })
      .addCase(deleteCloudDocument.rejected, (state, action) => {
        const message = action.payload as { title: string; subtitle: string };
        state.ui.announcements.push({ message });
      })
      .addCase(deleteCloudRevision.fulfilled, (state, action) => {
        const { id, documentId } = action.payload;
        const userDocument = state.documents.find(
          (doc) => doc.id === documentId
        );
        if (!userDocument) return;
        const cloudDocument = userDocument.cloud;
        if (!cloudDocument) return;
        const revision = cloudDocument.revisions.find(
          (revision) => revision.id === id
        );
        if (!revision) return;
        cloudDocument.revisions = cloudDocument.revisions.filter(
          (revision) => revision.id !== id
        );
      })
      .addCase(deleteCloudRevision.rejected, (state, action) => {
        const message = action.payload as { title: string; subtitle: string };
        state.ui.announcements.push({ message });
      })
      .addCase(updateUser.fulfilled, (state, action) => {
        const user = action.payload;
        state.user = user;
      })
      .addCase(updateUser.rejected, (state, action) => {
        const message = action.payload as { title: string; subtitle: string };
        state.ui.announcements.push({ message });
      })
      .addCase(alert.pending, (state, action) => {
        const alert = action.meta.arg;
        state.ui.alerts.push(alert);
      })
      .addCase(alert.fulfilled, (state) => {
        state.ui.alerts.shift();
      })
      .addCase(alert.rejected, (state, action) => {
        state.ui.alerts.shift();
        const message = action.payload as { title: string; subtitle: string };
        state.ui.announcements.push({ message });
      });
  },
});

export default appSlice.reducer;
