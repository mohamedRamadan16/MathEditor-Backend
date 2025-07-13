"use client";
import * as React from "react";
import { UserDocumentRevision, EditorDocumentRevision } from "@/types";
import { memo } from "react";
import { SxProps, Theme } from "@mui/material/styles";
import {
  Card,
  CardActionArea,
  CardHeader,
  Avatar,
  CardActions,
  Chip,
  IconButton,
} from "@mui/material";
import {
  Cloud,
  CloudSync,
  CloudUpload,
  Delete,
  DeleteForever,
  History,
  MobileFriendly,
  Update,
} from "@mui/icons-material";
import { actions, useDispatch, useSelector } from "@/store";
import { CLEAR_HISTORY_COMMAND, type LexicalEditor } from "lexical";
import useOnlineStatus from "@/hooks/useOnlineStatus";
import NProgress from "nprogress";
import { v4 as uuid } from "uuid";

const RevisionCard: React.FC<{
  revision: UserDocumentRevision;
  editorRef: React.RefObject<LexicalEditor | null>;
  sx?: SxProps<Theme> | undefined;
}> = memo(({ revision, editorRef, sx }) => {
  const dispatch = useDispatch();
  const user = useSelector((state) => state.user);
  const isOnline = useOnlineStatus();
  const userDocument = useSelector((state) =>
    state.documents.find((d) => d.id === revision.documentId)
  );
  const localDocument = userDocument?.local;
  const cloudDocument = userDocument?.cloud;
  const isLocalDocument = !!localDocument;
  const isCloudDocument = !!cloudDocument;
  const localDocumentRevisions = localDocument?.revisions ?? [];
  const cloudDocumentRevisions = cloudDocument?.revisions ?? [];
  const localRevision = localDocumentRevisions.find(
    (r) => r.id === revision.id
  );
  const isLocalRevision = !!localRevision;
  const cloudRevision = cloudDocumentRevisions.find(
    (r) => r.id === revision.id
  );
  const isCloudRevision = !!cloudRevision;
  const isLocalHead = isLocalDocument && localDocument.head === revision.id;
  const isCloudHead =
    isCloudDocument && isCloudRevision && cloudDocument.head === revision.id;
  const isCloudOnlyRevision = isCloudRevision && !isLocalRevision;
  const isLastCopy = isCloudOnlyRevision || isCloudOnlyRevision;
  const isHeadLocalRevision = localDocumentRevisions.some(
    (r) => r.id === localDocument?.head
  );
  const isHeadCloudRevision = cloudDocumentRevisions.some(
    (r) => r.id === localDocument?.head
  );
  const unsavedChanges = !isHeadLocalRevision && !isHeadCloudRevision;

  const isDocumentAuthor = isCloudDocument
    ? user?.id === cloudDocument.author.id
    : true;
  const isRevisionAuthor = isCloudRevision
    ? user?.id === cloudRevision.author.id
    : true;
  const diff = useSelector((state) => state.ui.diff);
  const showLocal = !diff.open && (isLocalRevision || isLocalHead);
  const showCloud = !diff.open && isCloudRevision;
  const showCreate = !diff.open && !isCloudRevision;
  const showUpdate =
    isOnline && isDocumentAuthor && isCloudRevision && !isCloudHead;
  const showDelete =
    !diff.open && isRevisionAuthor && !isLocalHead && !isCloudHead;
  const showDiff = diff.open;
  const isOld = diff.old === revision.id;
  const isNew = diff.new === revision.id;

  const setAsOld = () => dispatch(actions.setDiff({ old: revision.id }));
  const setAsNew = () => dispatch(actions.setDiff({ new: revision.id }));

  const getEditorDocumentRevision = async () => {
    const localResponse = await dispatch(actions.getLocalRevision(revision.id));
    if (localResponse.type === actions.getLocalRevision.fulfilled.type) {
      const editorDocumentRevision = localResponse.payload as ReturnType<
        typeof actions.getLocalRevision.fulfilled
      >["payload"];
      return editorDocumentRevision;
    } else {
      const cloudResponse = await dispatch(
        actions.getCloudRevision(revision.id)
      );
      if (cloudResponse.type === actions.getCloudRevision.fulfilled.type) {
        const editorDocumentRevision = cloudResponse.payload as ReturnType<
          typeof actions.getCloudRevision.fulfilled
        >["payload"];
        dispatch(actions.createLocalRevision(editorDocumentRevision));
        return editorDocumentRevision;
      }
    }
  };

  const getLocalEditorData = () => {
    console.log("Getting local editor data...");
    console.log("EditorRef exists:", !!editorRef.current);

    if (!editorRef.current) {
      console.error("Editor ref is null");
      return null;
    }

    // Check if the editor is properly initialized
    try {
      const editorState = editorRef.current.getEditorState();
      console.log("Editor state exists:", !!editorState);

      if (!editorState) {
        console.error("Editor state is null");
        return null;
      }

      const jsonData = editorState.toJSON();
      console.log("JSON data structure:", {
        hasData: !!jsonData,
        hasRoot: !!jsonData?.root,
        hasRootChildren: !!jsonData?.root?.children,
        rootChildrenCount: jsonData?.root?.children?.length || 0,
      });

      // Validate the structure
      if (!jsonData || !jsonData.root || !jsonData.root.children) {
        console.error("Invalid editor state structure:", jsonData);
        return null;
      }

      console.log("Valid JSON data obtained:", jsonData);
      return jsonData;
    } catch (error) {
      console.error("Error getting editor data:", error);
      return null;
    }
  };

  const createLocalRevision = async () => {
    if (!localDocument) return;
    const data = getLocalEditorData();
    if (!data) return;
    const payload = {
      id: localDocument.head,
      documentId: localDocument.id,
      createdAt: localDocument.updatedAt,
      data,
    };
    const response = await dispatch(actions.createLocalRevision(payload));
    if (response.type === actions.createLocalRevision.rejected.type) return;
    return response.payload as ReturnType<
      typeof actions.createLocalRevision.fulfilled
    >["payload"];
  };

  const createRevision = async () => {
    console.log("CreateRevision called");
    console.log("User:", user);
    console.log("Revision document ID:", revision.documentId);
    console.log("Is online:", isOnline);
    console.log("Unsaved changes:", unsavedChanges);

    if (unsavedChanges) await createLocalRevision();
    if (!isOnline) {
      dispatch(
        actions.announce({
          message: {
            title: "You are offline",
            subtitle: "Please connect to the internet to save to cloud storage",
          },
          action: { label: "Reload", onClick: "window.location.reload()" },
        })
      );
      return;
    }
    if (!user) {
      dispatch(
        actions.announce({
          message: {
            title: "You are not signed in",
            subtitle: "Please sign in to save your revision to the cloud",
          },
          action: { label: "Login", onClick: "login()" },
        })
      );
      return;
    }

    // Wait a bit for editor to be fully initialized and try multiple times
    let currentEditorData = null;
    let attempts = 0;
    const maxAttempts = 5;

    while (!currentEditorData && attempts < maxAttempts) {
      console.log(`Attempt ${attempts + 1} to get editor data`);
      await new Promise((resolve) => setTimeout(resolve, 100 * (attempts + 1))); // Increasing delay
      currentEditorData = getLocalEditorData();
      attempts++;
    }

    console.log("Final editor data obtained:", {
      hasData: !!currentEditorData,
      attempts: attempts,
      dataStructure: currentEditorData ? Object.keys(currentEditorData) : null,
      root: currentEditorData?.root,
      rootChildren: currentEditorData?.root?.children,
      rootChildrenLength: currentEditorData?.root?.children?.length,
      fullData: currentEditorData,
    });
    console.log("Current editor data obtained:", {
      hasData: !!currentEditorData,
      dataStructure: currentEditorData ? Object.keys(currentEditorData) : null,
      root: currentEditorData?.root,
      rootChildren: currentEditorData?.root?.children,
      rootChildrenLength: currentEditorData?.root?.children?.length,
      fullData: currentEditorData,
    });

    if (!currentEditorData) {
      console.error("No editor data available");
      dispatch(
        actions.announce({
          message: {
            title: "Unable to save",
            subtitle: "Could not capture current editor state",
          },
        })
      );
      return;
    }

    // Check if the data structure is valid
    if (!currentEditorData.root || !currentEditorData.root.children) {
      console.error("Invalid editor data structure:", currentEditorData);
      dispatch(
        actions.announce({
          message: {
            title: "Unable to save",
            subtitle: "Invalid editor state structure",
          },
        })
      );
      return;
    }

    const newRevision: EditorDocumentRevision = {
      id: "", // Will be set by the backend
      documentId: revision.documentId,
      data: currentEditorData,
      createdAt: new Date().toISOString(),
    };

    console.log("Creating revision with:", {
      documentId: revision.documentId,
      hasData: !!currentEditorData,
      isCloudDocument,
      isLocalDocument,
    });

    // If we have a cloud document, we should always create a revision, not a new document
    if (isCloudDocument) {
      const response = await dispatch(actions.createCloudRevision(newRevision));
      if (response.type === actions.createCloudRevision.rejected.type) return;
      return response.payload as ReturnType<
        typeof actions.createCloudRevision.fulfilled
      >["payload"];
    }
    // Only create a new document if we have no cloud document (local-only document)
    if (isLocalDocument && !isCloudDocument) {
      const editorDocument = {
        ...localDocument,
        data: newRevision.data,
        revisions: [],
      };
      return dispatch(actions.createCloudDocument(editorDocument));
    }
  };

  const viewRevision = async () => {
    NProgress.start();
    if (unsavedChanges) await createLocalRevision();
    if (diff.open)
      dispatch(actions.setDiff({ old: revision.id, new: revision.id }));
    const editorDocumentRevision = await getEditorDocumentRevision();
    if (!editorDocumentRevision) return NProgress.done();
    const editor = editorRef.current;
    if (!editor) return NProgress.done();
    const state = editor.parseEditorState(editorDocumentRevision.data);
    const payload = {
      id: editorDocumentRevision.documentId,
      partial: {
        head: editorDocumentRevision.id,
        updatedAt: editorDocumentRevision.createdAt,
      },
    };
    editor.update(() => {
      editor.setEditorState(state, { tag: JSON.stringify(payload) });
      editor.dispatchCommand(CLEAR_HISTORY_COMMAND, undefined);
      NProgress.done();
    });
  };

  const updateCloudHead = async () => {
    if (!isLocalHead) viewRevision();
    const payload = {
      id: revision.documentId,
      partial: { head: revision.id, updatedAt: revision.createdAt },
    };
    await dispatch(actions.updateCloudDocument(payload));
  };

  const deleteRevision = async () => {
    const variant = isLocalRevision ? "Local" : "Cloud";
    const alert = {
      title: `Delete ${variant} Revision?`,
      content: `Are you sure you want to delete this ${variant} revision?`,
      actions: [
        { label: "Cancel", id: uuid() },
        { label: "Delete", id: uuid() },
      ],
    };
    const response = await dispatch(actions.alert(alert));
    if (response.payload === alert.actions[1].id) {
      if (isLocalRevision)
        dispatch(
          actions.deleteLocalRevision({
            id: revision.id,
            documentId: revision.documentId,
          })
        );
      else
        dispatch(
          actions.deleteCloudRevision({
            id: revision.id,
            documentId: revision.documentId,
          })
        );
    }
  };

  return (
    <Card
      variant="outlined"
      sx={{
        display: "flex",
        flexDirection: "column",
        justifyContent: "space-between",
        height: "100%",
        maxWidth: "100%",
        ...sx,
      }}
    >
      <CardActionArea sx={{ flexGrow: 1 }} onClick={viewRevision}>
        <CardHeader
          sx={{
            alignItems: "start",
            "& .MuiCardHeader-content": {
              overflow: "hidden",
              textOverflow: "ellipsis",
            },
          }}
          title={new Date(revision.createdAt).toLocaleString(undefined, {
            dateStyle: "medium",
            timeStyle: "short",
          })}
          subheader={(cloudRevision?.author ?? user)?.name ?? "Local User"}
          avatar={
            <Avatar
              sx={{ bgcolor: "primary.main" }}
              src={(cloudRevision?.author ?? user)?.image ?? undefined}
              alt={(cloudRevision?.author ?? user)?.name}
            ></Avatar>
          }
        />
      </CardActionArea>
      <CardActions
        sx={{
          "& button:first-of-type": { ml: "auto !important" },
          "& .MuiChip-root:last-of-type": { mr: 1 },
        }}
      >
        {showLocal && (
          <Chip
            color={isLocalHead ? "primary" : "default"}
            sx={{ width: 0, flex: 1, maxWidth: "fit-content" }}
            icon={<MobileFriendly />}
            label="Local"
          />
        )}
        {showCloud && (
          <Chip
            color={isCloudHead ? "primary" : "default"}
            sx={{ width: 0, flex: 1, maxWidth: "fit-content" }}
            icon={<Cloud />}
            label="Cloud"
          />
        )}
        {showDiff && (
          <Chip
            color={isOld ? "primary" : "default"}
            sx={{ width: 0, flex: 1, maxWidth: "fit-content" }}
            icon={<History />}
            label="Old"
            onClick={setAsOld}
            disabled={!isOnline && !isLocalRevision}
          />
        )}
        {showDiff && (
          <Chip
            color={isNew ? "primary" : "default"}
            sx={{ width: 0, flex: 1, maxWidth: "fit-content" }}
            icon={<Update />}
            label="New"
            onClick={setAsNew}
            disabled={!isOnline && !isLocalRevision}
          />
        )}
        {showCreate && (
          <Chip
            variant="outlined"
            clickable
            sx={{ width: 0, flex: 1, maxWidth: "fit-content" }}
            icon={<CloudUpload />}
            label="Save to Cloud"
            onClick={createRevision}
          />
        )}
        {showUpdate && (
          <IconButton
            aria-label="Update Cloud Head"
            size="small"
            onClick={updateCloudHead}
          >
            <CloudSync />
          </IconButton>
        )}
        {showDelete && (
          <IconButton
            aria-label="Delete Revision"
            size="small"
            onClick={deleteRevision}
          >
            {isLastCopy ? <DeleteForever /> : <Delete />}
          </IconButton>
        )}
      </CardActions>
    </Card>
  );
});

export default RevisionCard;
