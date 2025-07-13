"use client";
import { useRouter } from "next/navigation";
import { useDispatch, useSelector, actions } from "@/store";
import React from "react";
import { Snackbar, Button, IconButton, Typography } from "@mui/material";
import { Close } from "@mui/icons-material";

function Announcer() {
  const announcement = useSelector((state) => state.ui.announcements[0]);
  const dispatch = useDispatch();
  const router = useRouter();
  const navigate = (path: string) => router.push(path);
  const login = () => {
    // Handle login - this might need to be updated based on your auth flow
    console.log("Login clicked from announcer - implement login flow");
  };

  const handleClose = () => dispatch(actions.clearAnnouncement());
  const handleConfirm = () => {
    const serializedAction = announcement?.action?.onClick;
    if (serializedAction) {
      const action = new Function(
        "dispatch",
        "actions",
        "navigate",
        "login",
        serializedAction
      );
      action.bind(null, dispatch, actions, navigate, login)();
    }
    dispatch(actions.clearAnnouncement());
  };

  if (!announcement) return null;
  if (!announcement.message) return null;

  const message = (
    <>
      <Typography variant="subtitle2">{announcement.message.title}</Typography>
      {announcement.message.subtitle ? announcement.message.subtitle : null}
    </>
  );

  return (
    <Snackbar
      open
      autoHideDuration={announcement.timeout ?? 5000}
      onClose={handleClose}
      message={message}
      action={
        announcement.action ? (
          <>
            <Button color="secondary" size="small" onClick={handleConfirm}>
              {announcement.action.label}
            </Button>
            <IconButton
              size="small"
              aria-label="close"
              color="inherit"
              onClick={handleClose}
            >
              <Close fontSize="small" />
            </IconButton>
          </>
        ) : null
      }
    />
  );
}

export default Announcer;
