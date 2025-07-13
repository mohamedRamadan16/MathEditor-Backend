"use client";
import StoreProvider from "@/store/StoreProvider";
import TopAppBar from "./TopAppBar";
import AlertDialog from "./Alert";
import Announcer from "./Announcer";
import ProgressBar from "./ProgressBar";
import AuthGuard from "@/components/AuthGuard";
import { Container } from "@mui/material";
import { Suspense } from "react";

const AppLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <>
      <Suspense>
        <ProgressBar />
      </Suspense>
      <StoreProvider>
        <AuthGuard>
          <TopAppBar />
          <Container
            className="editor-container"
            sx={{
              display: "flex",
              flexDirection: "column",
              mx: "auto",
              my: 2,
              flex: 1,
              position: "relative",
            }}
          >
            {children}
          </Container>
          <AlertDialog />
          <Announcer />
        </AuthGuard>
      </StoreProvider>
    </>
  );
};

export default AppLayout;
