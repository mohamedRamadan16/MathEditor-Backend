"use client";
import {
  $addUpdateTag,
  $getPreviousSelection,
  $getSelection,
  $isRangeSelection,
  $setSelection,
  BLUR_COMMAND,
  CLICK_COMMAND,
  COMMAND_PRIORITY_CRITICAL,
  KEY_DOWN_COMMAND,
  LexicalEditor,
  LexicalNode,
  SELECTION_CHANGE_COMMAND,
  SerializedParagraphNode,
} from "lexical";
import { mergeRegister } from "@lexical/utils";
import { useCallback, useEffect, useRef, useState } from "react";
import {
  Menu,
  Button,
  MenuItem,
  ListItemText,
  Typography,
  TextField,
  CircularProgress,
  IconButton,
  ListItemIcon,
} from "@mui/material";
import {
  AutoAwesome,
  UnfoldMore,
  UnfoldLess,
  PlayArrow,
  ImageSearch,
  Autorenew,
  ArrowDropDown,
  ArrowDropUp,
  Send,
  Settings,
} from "@mui/icons-material";
import { SxProps, Theme } from "@mui/material/styles";
import { useCompletion } from "@ai-sdk/react";
import { SET_DIALOGS_COMMAND } from "../Dialogs/commands";
import { ANNOUNCE_COMMAND, UPDATE_DOCUMENT_COMMAND } from "@/editor/commands";
import { Announcement } from "@/types";
import { throttle } from "@/editor/utils/throttle";
import {
  $convertFromMarkdownString,
  createTransformers,
} from "../../MarkdownPlugin";
import { createHeadlessEditor } from "@lexical/headless";
import { $generateNodesFromSerializedNodes } from "@lexical/clipboard";

const getLlmConfig = () => {
  const initialValue = { provider: "google", model: "gemini-2.0-flash" };
  try {
    const item = window.localStorage.getItem("llm");
    return item ? JSON.parse(item) : initialValue;
  } catch (error) {
    console.log(error);
    return initialValue;
  }
};

const serializedParagraph: SerializedParagraphNode = {
  children: [],
  direction: null,
  format: "",
  indent: 0,
  type: "paragraph",
  version: 1,
  textFormat: 0,
  textStyle: "",
};

export default function AITools({
  editor,
  sx,
}: {
  editor: LexicalEditor;
  sx?: SxProps<Theme>;
}) {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);
  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };
  const handleClose = useCallback(() => {
    setAnchorEl(null);
    setTimeout(() => {
      editor.update(
        () => {
          const selection = $getSelection() || $getPreviousSelection();
          if (!selection) return;
          $setSelection(selection.clone());
        },
        {
          discrete: true,
          onUpdate() {
            editor.focus(undefined, { defaultSelection: "rootStart" });
          },
        }
      );
    }, 0);
  }, [editor]);

  const promptRef = useRef<HTMLTextAreaElement>(null);

  // State for direct completion handling
  const [isDirectLoading, setIsDirectLoading] = useState(false);

  // useCompletion hook for zap (custom command) functionality
  const {
    completion,
    complete,
    isLoading: isZapLoading,
    stop,
  } = useCompletion({
    api: "/api/completion",
    onError(error) {
      console.error("Zap completion error:", error);
      annouunce({
        message: {
          title: "AI completion failed",
          subtitle: "Please try again later",
        },
      });
    },
    onFinish(prompt, completion) {
      if (completion && completion.length > 0) {
        insertTextIntoEditor(completion);
      }
    },
  });

  // Combined loading state
  const isLoading = isZapLoading || isDirectLoading;

  // Helper function to insert text into the editor
  const insertTextIntoEditor = (text: string) => {
    editor.update(
      () => {
        const selection = $getSelection();

        if (!$isRangeSelection(selection)) {
          return;
        }

        // Reset offset since we're handling the complete text
        offsetRef.current = 0;

        // Insert the completion text at the current cursor position
        selection.insertText(text);
      },
      {
        onUpdate: updateDocument,
      }
    );
  };

  // Direct completion function for all AI completion options
  const performDirectCompletion = async (prompt: string, option: string) => {
    setIsDirectLoading(true);
    try {
      const response = await fetch("/api/completion", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          prompt,
          option,
          provider: undefined,
          model: undefined,
        }),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP ${response.status}: ${errorText}`);
      }

      const result = await response.text();
      if (result && result.length > 0) {
        insertTextIntoEditor(result);
      }
    } catch (error) {
      console.error("AI completion failed:", error);
      annouunce({
        message: {
          title: "AI completion failed",
          subtitle:
            error instanceof Error ? error.message : "Please try again later",
        },
      });
    } finally {
      setIsDirectLoading(false);
    }
  };

  const annouunce = useCallback(
    (announcement: Announcement) => {
      editor.dispatchCommand(ANNOUNCE_COMMAND, announcement);
    },
    [editor]
  );

  const [isCollapsed, setIsCollapsed] = useState(true);
  const offsetRef = useRef(0);

  const handlePrompt = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    const textarea = e.currentTarget;
    const isNavigatingUp = textarea.selectionStart === 0 && e.key === "ArrowUp";
    const isNavigatingDown =
      textarea.selectionStart === textarea.value.length &&
      e.key === "ArrowDown";
    if (!isNavigatingUp && !isNavigatingDown) e.stopPropagation();
    if (isNavigatingDown) textarea.closest("li")?.focus();
    const command = textarea.value;
    const isSubmit =
      e.key === "Enter" && !e.shiftKey && command.trim().length > 0;
    if (!isSubmit) return;
    e.preventDefault();
    handleSubmit();
  };

  const handleSubmit = () => {
    const command = promptRef.current?.value;
    if (!command) return;
    handleClose();
    editor.getEditorState().read(() => {
      const selection = $getSelection();
      if (!$isRangeSelection(selection)) return;
      const anchorNode = selection.anchor.getNode();
      let currentNode: LexicalNode | null | undefined = anchorNode;
      let textContent = "";
      while (currentNode && textContent.length < 1024) {
        textContent = currentNode.getTextContent() + "\n\n" + textContent;
        currentNode =
          currentNode.getPreviousSibling() ||
          currentNode.getParent()?.getPreviousSibling();
      }
      const { provider, model } = getLlmConfig();
      complete(textContent, {
        body: { option: "zap", command, provider, model },
      });
    });
  };

  const handleRewrite = () => {
    handleClose();
    editor.getEditorState().read(() => {
      const selection = $getSelection();
      if (!$isRangeSelection(selection)) return;
      const textContent = selection.getTextContent();
      performDirectCompletion(textContent, "improve");
    });
  };

  const handleShorter = () => {
    handleClose();
    editor.getEditorState().read(() => {
      const selection = $getSelection();
      if (!$isRangeSelection(selection)) return;
      const textContent = selection.getTextContent();
      performDirectCompletion(textContent, "shorter");
    });
  };

  const handleLonger = () => {
    handleClose();
    editor.getEditorState().read(() => {
      const selection = $getSelection();
      if (!$isRangeSelection(selection)) return;
      const textContent = selection.getTextContent();
      performDirectCompletion(textContent, "longer");
    });
  };

  const handleContinue = () => {
    handleClose();

    const editorState = editor.getEditorState();
    editorState.read(() => {
      const selection = $getSelection();

      if (!$isRangeSelection(selection)) {
        return;
      }

      const anchorNode = selection.anchor.getNode();
      let currentNode: LexicalNode | null | undefined = anchorNode;
      let textContent = "";

      while (currentNode && textContent.length < 1024) {
        textContent = currentNode.getTextContent() + "\n\n" + textContent;
        currentNode =
          currentNode.getPreviousSibling() ||
          currentNode.getParent()?.getPreviousSibling();
      }

      // Position cursor at the end if text is selected
      const isCollapsed = selection.isCollapsed();
      if (!isCollapsed) {
        editor.update(() => {
          const newSelection = $getSelection();
          if ($isRangeSelection(newSelection)) {
            (newSelection.isBackward()
              ? newSelection.anchor
              : newSelection.focus
            )
              .getNode()
              .selectEnd();
          }
        });
      }

      performDirectCompletion(textContent, "continue");
    });
  };

  const handleOCR = () => {
    handleClose();
    editor.dispatchCommand(SET_DIALOGS_COMMAND, { ocr: { open: true } });
  };

  const convertMarkdownToJSON = useCallback(
    (markdown: string) => {
      const transformers = createTransformers(editor);
      const nodes = Array.from(editor._nodes.values()).map(
        (registry) => registry.klass
      );
      const config = { nodes };
      const headlessEditor = createHeadlessEditor(config);
      headlessEditor.update(
        () => {
          $convertFromMarkdownString(markdown, transformers);
        },
        { discrete: true }
      );
      return headlessEditor.getEditorState().toJSON();
    },
    [editor]
  );

  useEffect(() => {
    console.log("=== LOADING STATE CHANGED ===", isLoading);
  }, [isLoading]);

  useEffect(() => {
    console.log("=== COMPLETION STATE CHANGED ===", completion.length, "chars");
  }, [completion]);

  useEffect(() => {
    // Disable progressive streaming since we handle everything in onFinish
    // This prevents interference and timing issues
    return;
  }, [completion, isLoading]);

  const updateDocument = useCallback(
    throttle(() => {
      editor.dispatchCommand(UPDATE_DOCUMENT_COMMAND, undefined);
    }, 1000),
    [editor]
  );

  useEffect(() => {
    return mergeRegister(
      editor.registerCommand(
        SELECTION_CHANGE_COMMAND,
        () => {
          if (isLoading) return false;
          const selection = $getSelection();
          setIsCollapsed(selection?.isCollapsed() ?? true);
          return false;
        },
        COMMAND_PRIORITY_CRITICAL
      ),
      editor.registerCommand(
        CLICK_COMMAND,
        () => {
          if (isLoading) stop();
          return false;
        },
        COMMAND_PRIORITY_CRITICAL
      ),
      editor.registerCommand(
        KEY_DOWN_COMMAND,
        () => {
          if (isLoading) stop();
          return false;
        },
        COMMAND_PRIORITY_CRITICAL
      ),
      editor.registerCommand(
        BLUR_COMMAND,
        () => {
          if (isLoading) stop();
          return false;
        },
        COMMAND_PRIORITY_CRITICAL
      )
    );
  }, [editor, isLoading, stop]);

  const openAiSettings = () => {
    editor.dispatchCommand(SET_DIALOGS_COMMAND, { ai: { open: true } });
  };

  return (
    <>
      <Button
        id="ai-tools-button"
        aria-controls={open ? "ai-tools-menu" : undefined}
        aria-haspopup="true"
        aria-expanded={open ? "true" : undefined}
        variant="outlined"
        onClick={handleClick}
        startIcon={<AutoAwesome color={isLoading ? "disabled" : "action"} />}
        endIcon={
          isLoading ? (
            <CircularProgress size={16} color="inherit" />
          ) : open ? (
            <ArrowDropUp color={isLoading ? "disabled" : "action"} />
          ) : (
            <ArrowDropDown color={isLoading ? "disabled" : "action"} />
          )
        }
        sx={{
          color: "text.primary",
          borderColor: "divider",
          p: 1,
          minWidth: 0,
          height: 36,
          "& .MuiButton-startIcon": { mr: { xs: 0, sm: 1 }, ml: 0 },
          "& .MuiButton-endIcon": { mr: 0, ml: isLoading ? 1 : 0 },
          "& .MuiButton-endIcon > svg": { fontSize: 20 },
        }}
        disabled={isLoading}
      >
        <Typography
          variant="button"
          sx={{ display: { xs: "none", sm: "block" } }}
        >
          AI
        </Typography>
      </Button>
      <Menu
        id="ai-tools-menu"
        aria-label="Formatting options for ai"
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        anchorOrigin={{
          vertical: "bottom",
          horizontal: "center",
        }}
        transformOrigin={{
          vertical: "top",
          horizontal: "center",
        }}
        sx={{
          "& .MuiList-root": { pt: 0 },
          "& .MuiBackdrop-root": { userSelect: "none" },
          "& .MuiMenuItem-root": { minHeight: 36 },
        }}
      >
        <MenuItem
          sx={{
            p: 0,
            mb: 1,
            flexDirection: "column",
            backgroundColor: "transparent !important",
          }}
          disableRipple
          disableTouchRipple
          onFocusVisible={(e) => {
            const currentTarget = e.currentTarget;
            const relatedTarget = e.relatedTarget;
            setTimeout(() => {
              const promptInput = promptRef.current;
              const isPromptFocused = document.activeElement === promptInput;
              if (isPromptFocused) return;
              if (relatedTarget !== promptInput) promptInput?.focus();
              else currentTarget.nextElementSibling?.focus();
            }, 0);
          }}
          disabled={isLoading}
        >
          <TextField
            multiline
            hiddenLabel
            variant="filled"
            size="small"
            placeholder="What to do?"
            inputRef={promptRef}
            autoFocus
            autoComplete="off"
            spellCheck="false"
            sx={{
              flexGrow: 1,
              width: 256,
              "& .MuiInputBase-root": { paddingRight: 9, flexGrow: 1 },
            }}
            slotProps={{
              htmlInput: {
                onKeyDown: handlePrompt,
              },
            }}
          />
          <ListItemIcon sx={{ position: "absolute", right: 4, bottom: 6 }}>
            <IconButton
              onClick={handleSubmit}
              disabled={isLoading}
              size="small"
            >
              <Send />
            </IconButton>
            <IconButton
              onClick={openAiSettings}
              disabled={isLoading}
              size="small"
            >
              <Settings />
            </IconButton>
          </ListItemIcon>
        </MenuItem>
        <MenuItem disabled={isLoading} onClick={handleContinue}>
          <ListItemIcon>
            <PlayArrow />
          </ListItemIcon>
          <ListItemText>Continue Writing</ListItemText>
        </MenuItem>
        <MenuItem disabled={isLoading || isCollapsed} onClick={handleRewrite}>
          <ListItemIcon>
            <Autorenew />
          </ListItemIcon>
          <ListItemText>Rewrite</ListItemText>
        </MenuItem>
        <MenuItem disabled={isLoading || isCollapsed} onClick={handleShorter}>
          <ListItemIcon>
            <UnfoldLess />
          </ListItemIcon>
          <ListItemText>Shorter</ListItemText>
        </MenuItem>
        <MenuItem disabled={isLoading || isCollapsed} onClick={handleLonger}>
          <ListItemIcon>
            <UnfoldMore />
          </ListItemIcon>
          <ListItemText>Longer</ListItemText>
        </MenuItem>
        <MenuItem disabled={isLoading || !isCollapsed} onClick={handleOCR}>
          <ListItemIcon>
            <ImageSearch />
          </ListItemIcon>
          <ListItemText>Image to Text</ListItemText>
        </MenuItem>
      </Menu>
    </>
  );
}
