"use client";

import React, { useState } from "react";
import {
  Box,
  Button,
  TextField,
  Typography,
  Paper,
  Tab,
  Tabs,
  Alert,
} from "@mui/material";
import { useAuth } from "@/hooks/useAuth";
import { LoginRequest, RegisterRequest } from "@/types/api";

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`auth-tabpanel-${index}`}
      aria-labelledby={`auth-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

export default function AuthComponent() {
  const [tabValue, setTabValue] = useState(0);
  const { login, register, isLoading, error } = useAuth();

  const [loginData, setLoginData] = useState<LoginRequest>({
    email: "",
    password: "",
  });

  const [registerData, setRegisterData] = useState<RegisterRequest>({
    name: "",
    email: "",
    password: "",
    handle: "",
  });

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await login(loginData);
    } catch (err) {
      console.error("Login error:", err);
    }
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await register(registerData);
    } catch (err) {
      console.error("Register error:", err);
    }
  };

  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        minHeight: "100vh",
        bgcolor: "background.default",
      }}
    >
      <Paper elevation={3} sx={{ width: "100%", maxWidth: 400, p: 2 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          MathEditor
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <Tabs
          value={tabValue}
          onChange={handleTabChange}
          aria-label="auth tabs"
        >
          <Tab label="Login" />
          <Tab label="Register" />
        </Tabs>

        <TabPanel value={tabValue} index={0}>
          <Box component="form" onSubmit={handleLogin} sx={{ mt: 2 }}>
            <TextField
              fullWidth
              label="Email"
              type="email"
              value={loginData.email}
              onChange={(e) =>
                setLoginData({ ...loginData, email: e.target.value })
              }
              margin="normal"
              required
            />
            <TextField
              fullWidth
              label="Password"
              type="password"
              value={loginData.password}
              onChange={(e) =>
                setLoginData({ ...loginData, password: e.target.value })
              }
              margin="normal"
              required
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              sx={{ mt: 3, mb: 2 }}
              disabled={isLoading}
            >
              {isLoading ? "Logging in..." : "Login"}
            </Button>
          </Box>
        </TabPanel>

        <TabPanel value={tabValue} index={1}>
          <Box component="form" onSubmit={handleRegister} sx={{ mt: 2 }}>
            <TextField
              fullWidth
              label="Name"
              value={registerData.name}
              onChange={(e) =>
                setRegisterData({ ...registerData, name: e.target.value })
              }
              margin="normal"
              required
            />
            <TextField
              fullWidth
              label="Handle"
              value={registerData.handle}
              onChange={(e) =>
                setRegisterData({ ...registerData, handle: e.target.value })
              }
              margin="normal"
              required
            />
            <TextField
              fullWidth
              label="Email"
              type="email"
              value={registerData.email}
              onChange={(e) =>
                setRegisterData({ ...registerData, email: e.target.value })
              }
              margin="normal"
              required
            />
            <TextField
              fullWidth
              label="Password"
              type="password"
              value={registerData.password}
              onChange={(e) =>
                setRegisterData({ ...registerData, password: e.target.value })
              }
              margin="normal"
              required
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              sx={{ mt: 3, mb: 2 }}
              disabled={isLoading}
            >
              {isLoading ? "Creating account..." : "Register"}
            </Button>
          </Box>
        </TabPanel>
      </Paper>
    </Box>
  );
}
