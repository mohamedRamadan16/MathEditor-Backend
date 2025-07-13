"use client";
import { RefObject, RefCallback } from "react";
import { COMMAND_PRIORITY_LOW } from "lexical";
import { mergeRegister } from "@lexical/utils";
import type { EditorDocument } from "@/types";
import {
  ANNOUNCE_COMMAND,
  UPDATE_DOCUMENT_COMMAND,
  ALERT_COMMAND,
} from "@/editor/commands";
import { actions, useDispatch } from "@/store";
import type { EditorState, LexicalEditor } from "lexical";
import Editor from "@/editor/Editor";

const DEFAULT_LEXICAL_STATE = {
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
    direction: "ltr" as const,
    format: "",
    indent: 0,
    type: "root",
    version: 1,
  },
};

const Container: React.FC<{
  document: EditorDocument;
  editorRef?: RefObject<LexicalEditor | null> | RefCallback<LexicalEditor>;
  onChange?: (
    editorState: EditorState,
    editor: LexicalEditor,
    tags: Set<string>
  ) => void;
  ignoreHistoryMerge?: boolean;
  readOnly?: boolean;
}> = ({
  document,
  editorRef,
  onChange,
  ignoreHistoryMerge,
  readOnly = false,
}) => {
  const dispatch = useDispatch();
  const editorRefCallback = (editor: LexicalEditor) => {
    if (typeof editorRef === "function") {
      editorRef(editor);
    } else if (typeof editorRef === "object") {
      editorRef.current = editor;
    }
    return mergeRegister(
      editor.registerCommand(
        ANNOUNCE_COMMAND,
        (payload) => {
          dispatch(actions.announce(payload));
          return false;
        },
        COMMAND_PRIORITY_LOW
      ),
      editor.registerCommand(
        ALERT_COMMAND,
        (payload) => {
          dispatch(actions.alert(payload));
          return false;
        },
        COMMAND_PRIORITY_LOW
      ),
      editor.registerCommand(
        UPDATE_DOCUMENT_COMMAND,
        () => {
          const editorState = editor.getEditorState();
          onChange?.(editorState, editor, new Set());
          return false;
        },
        COMMAND_PRIORITY_LOW
      )
    );
  };

  // Validate document.data
  let editorStateData = document.data;
  console.log("[Lexical Debug] Raw document.data:", document.data);
  if (
    !editorStateData ||
    typeof editorStateData !== "object" ||
    !("root" in editorStateData) ||
    !Array.isArray(editorStateData.root.children) ||
    editorStateData.root.children.length === 0
  ) {
    editorStateData = DEFAULT_LEXICAL_STATE as typeof document.data;
    console.warn(
      "[Lexical Debug] Using default Lexical state due to missing/invalid data."
    );
  }
  console.log(
    "[Lexical Debug] Final editorStateData passed to Lexical:",
    editorStateData
  );

  return (
    <Editor
      initialConfig={{ editorState: JSON.stringify(editorStateData) }}
      onChange={onChange}
      editorRef={editorRefCallback}
      ignoreHistoryMerge={ignoreHistoryMerge}
      readOnly={readOnly}
    />
  );
};

export default Container;
