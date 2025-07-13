// Simple test to verify login functionality
const API_URL = "http://localhost:5041/api";

async function testLogin() {
  try {
    console.log("Testing login with test user...");

    const response = await fetch(`${API_URL}/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email: "test@example.com",
        password: "Test123!",
      }),
    });

    console.log("Response status:", response.status);

    if (!response.ok) {
      const error = await response.json();
      console.error("Login failed:", error);
      return;
    }

    const data = await response.json();
    console.log("Login successful!");
    console.log("Token received:", data.token.substring(0, 20) + "...");

    // Test session endpoint with token
    const sessionResponse = await fetch(`${API_URL}/auth/session`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${data.token}`,
        "Content-Type": "application/json",
      },
    });

    if (sessionResponse.ok) {
      const sessionData = await sessionResponse.json();
      console.log("Session data:", sessionData);
    } else {
      console.error("Session check failed:", sessionResponse.status);
    }
  } catch (error) {
    console.error("Test failed:", error);
  }
}

testLogin();
