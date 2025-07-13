"use client";

import React, { useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useAuth } from "@/hooks/useAuth";
import { RootState } from "@/store";
import { load } from "@/store/app";
import AuthComponent from "@/components/AuthComponent";
import { Box, CircularProgress, Typography } from "@mui/material";

interface AuthGuardProps {
  children: React.ReactNode;
}

export default function AuthGuard({ children }: AuthGuardProps) {
  const dispatch = useDispatch();
  const { isAuthenticated } = useAuth();
  const user = useSelector((state: RootState) => state.user);
  const ui = useSelector((state: RootState) => state.ui);

  useEffect(() => {
    // Load initial data when component mounts
    dispatch(load() as any);
  }, [dispatch]);

  // Show loading while initializing
  if (!ui.initialized) {
    return (
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          alignItems: "center",
          minHeight: "100vh",
          gap: 2,
        }}
      >
        <CircularProgress />
        <Typography variant="body1">Loading...</Typography>
      </Box>
    );
  }

  // Show auth component if not authenticated or no user
  if (!isAuthenticated() || !user) {
    return <AuthComponent />;
  }

  // Show main app if authenticated
  return <>{children}</>;
}
