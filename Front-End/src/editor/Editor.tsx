"use client";
import type { EditorState, LexicalEditor } from "lexical";
import {
  LexicalComposer,
  InitialConfigType,
} from "@lexical/react/LexicalComposer";
import { SharedHistoryContext } from "./context/SharedHistoryContext";
import { ContentEditable } from "@lexical/react/LexicalContentEditable";
import ToolbarPlugin from "./plugins/ToolbarPlugin";
import { editorConfig } from "./config";
import { EditorPlugins } from "./plugins";
import { MutableRefObject, RefCallback } from "react";
import { EditorRefPlugin } from "@lexical/react/LexicalEditorRefPlugin";

export const Editor: React.FC<{
  initialConfig: Partial<InitialConfigType>;
  editorRef:
    | MutableRefObject<LexicalEditor | null>
    | RefCallback<LexicalEditor>;
  onChange?: (
    editorState: EditorState,
    editor: LexicalEditor,
    tags: Set<string>
  ) => void;
  ignoreHistoryMerge?: boolean;
  readOnly?: boolean;
}> = ({
  initialConfig,
  onChange,
  editorRef,
  ignoreHistoryMerge,
  readOnly = false,
}) => {
  return (
    <LexicalComposer
      initialConfig={{ ...editorConfig, ...initialConfig, editable: !readOnly }}
    >
      <SharedHistoryContext>
        {!readOnly && <ToolbarPlugin />}
        <EditorPlugins
          onChange={onChange}
          ignoreHistoryMerge={ignoreHistoryMerge}
          contentEditable={
            <ContentEditable
              className="editor-input"
              ariaLabel="editor input"
            />
          }
        />
        <EditorRefPlugin editorRef={editorRef} />
      </SharedHistoryContext>
    </LexicalComposer>
  );
};

export default Editor;
