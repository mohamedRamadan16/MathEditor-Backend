"use client";
import { useDispatch, actions, useSelector } from "@/store";
import { UserDocument } from "@/types";
import { Public, PublicOff } from "@mui/icons-material";
import {
  IconButton,
  MenuItem,
  ListItemIcon,
  ListItemText,
  Tooltip,
} from "@mui/material";

const TogglePublished: React.FC<{
  userDocument: UserDocument;
  variant?: "menuitem" | "iconbutton";
  closeMenu?: () => void;
}> = ({ userDocument, variant = "iconbutton", closeMenu }) => {
  const dispatch = useDispatch();
  const user = useSelector((state) => state.user);
  const cloudDocument = userDocument?.cloud;
  const isCloud = !!cloudDocument;
  const isPublished = isCloud && cloudDocument.published;
  const isAuthor = isCloud && cloudDocument.author.id === user?.id;

  const handleTogglePublished = () => {
    if (!isCloud || !isAuthor) return;

    console.log(
      "Toggling published status for document:",
      cloudDocument.id,
      "from",
      isPublished,
      "to",
      !isPublished
    );

    dispatch(
      actions.toggleDocumentPublished({
        id: cloudDocument.id,
        published: !isPublished,
      })
    );

    if (closeMenu) closeMenu();
  };

  if (!isCloud || !isAuthor) return null;

  if (variant === "menuitem") {
    return (
      <MenuItem onClick={handleTogglePublished}>
        <ListItemIcon>{isPublished ? <PublicOff /> : <Public />}</ListItemIcon>
        <ListItemText>{isPublished ? "Unpublish" : "Publish"}</ListItemText>
      </MenuItem>
    );
  }

  return (
    <Tooltip title={isPublished ? "Unpublish Document" : "Publish Document"}>
      <IconButton
        onClick={handleTogglePublished}
        size="small"
        aria-label={isPublished ? "Unpublish Document" : "Publish Document"}
      >
        {isPublished ? <PublicOff /> : <Public />}
      </IconButton>
    </Tooltip>
  );
};

export default TogglePublished;
