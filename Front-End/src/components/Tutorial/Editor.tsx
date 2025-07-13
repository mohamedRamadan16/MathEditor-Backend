"use client"
import { useCallback } from "react";
import { checkpoints } from "./checkpoints";
import tutorialTemplate from './tutorial.json';
import { $addUpdateTag, $getRoot, $getSelection, $isRangeSelection, COMMAND_PRIORITY_NORMAL, DELETE_CHARACTER_COMMAND, EditorState, LexicalEditor, LexicalNode } from "lexical";
import { $isDetailsContainerNode, $isDetailsContentNode, $isDetailsSummaryNode } from "@/editor/nodes/DetailsNode";
import { $isListNode, $isListItemNode } from "@lexical/list";
import { PropsWithChildren } from 'react';
import { EditorDocument } from '@/types';
import Editor from "../Editor";

const document = tutorialTemplate as unknown as EditorDocument;

const TutorialEditor: React.FC<PropsWithChildren> = ({ children }) => {

  const onChange = (editorState: EditorState, editor: LexicalEditor, tags: Set<string>) => {
    if (tags.has('checkpoint')) return;
    editor.update(() => {
      const selection = $getSelection();
      if (!selection) return;
      const selectedNode = selection.getNodes()[0];
      if (!selectedNode) return;
      const root = $getRoot();
      const testSuites = root.getChildren().filter($isDetailsContainerNode);
      for (let i = 0; i < testSuites.length; i++) {
        const testSuite = testSuites[i];
        if (!testSuite.isParentOf(selectedNode)) continue;
        const content = testSuite.getChildren().find($isDetailsContentNode);
        if (!content) continue;
        const tasks = content.getChildren().filter($isDetailsContainerNode);
        for (let j = 0; j < tasks.length; j++) {
          const task = tasks[j];
          const nextSibling = task.getNextSibling();
          if (!nextSibling) continue;
          const nextSiblings = nextSibling.getNodesBetween(tasks[j + 1] ?? testSuites[i + 1] ?? root);
          if (!nextSiblings.some(node => node.is(selectedNode) || node.isParentOf(selectedNode))) continue;
          const checkList = task.getChildren()?.find($isDetailsSummaryNode)?.getFirstChild();
          if (!$isListNode(checkList)) continue;
          const checkListItem = checkList.getFirstChild();
          if (!$isListItemNode(checkListItem)) continue;
          const checked = checkpoints[i][j](nextSibling);
          checkListItem.setChecked(checked);
          $addUpdateTag('history-merge');
        };
      }
    }, { discrete: true, tag: 'checkpoint' })
  };

  const registerListeners = useCallback((editor: LexicalEditor) => {
    if (!editor) return;
    return editor.registerCommand(
      DELETE_CHARACTER_COMMAND,
      (isBackward) => {
        const selection = $getSelection();
        if (!$isRangeSelection(selection)) {
          return false;
        }
        if (!selection.isCollapsed()) {
          const selectionNode = selection.getNodes();
          if (selectionNode.some($isDetailsContainerNode)) {
            return true;
          }
          return false;
        }

        const anchorNode = selection.anchor.getNode();
        const isAtStart = selection.anchor.offset === 0;
        const isAtEnd = selection.anchor.offset === anchorNode.getTextContentSize();
        const isAtStartOrEnd = isBackward ? isAtStart : isAtEnd;
        if (!isAtStartOrEnd) {
          return false;
        }
        const topLevelElement = anchorNode.getTopLevelElement();
        if (topLevelElement === null) {
          return false;
        }
        const previousSibling = topLevelElement.getPreviousSibling<LexicalNode>();
        const nextSibling = topLevelElement.getNextSibling<LexicalNode>();
        const container = isBackward ? previousSibling : nextSibling;
        return $isDetailsContainerNode(container);
      },
      COMMAND_PRIORITY_NORMAL,
    );

  }, []);

  return (
    <Editor document={document} onChange={onChange} editorRef={registerListeners} ignoreHistoryMerge={false} />
  );
}

export default TutorialEditor;