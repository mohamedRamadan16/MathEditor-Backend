const API_BASE_URL = "http://localhost:5041/api";

// Test data - similar to what the frontend would send
const testRevisionData = {
  documentId: "c025a715-6953-4fef-9fe6-9b959d797ede", // Using the actual document ID
  data: {
    root: {
      children: [
        {
          children: [
            {
              detail: 0,
              format: "", // String, not integer
              mode: "normal",
              style: "",
              text: "Hello World",
              type: "text",
              version: 1,
            },
          ],
          direction: "ltr",
          format: "", // String, not integer
          indent: 0,
          type: "paragraph",
          version: 1,
        },
      ],
      direction: "ltr",
      format: "", // String, not integer
      indent: 0,
      type: "root",
      version: 1,
    },
  },
};

async function testRevisionCreation() {
  try {
    console.log("Testing revision creation...");

    // First, login to get a token
    const loginResponse = await fetch(`${API_BASE_URL}/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email: "test@example.com",
        password: "Test123!",
      }),
    });

    if (!loginResponse.ok) {
      console.error("Login failed:", await loginResponse.text());
      return;
    }

    const loginData = await loginResponse.json();
    console.log("Login successful:", loginData);

    const token = loginData.token; // Direct access to token

    // Now try to create a revision
    console.log(
      "Sending revision data:",
      JSON.stringify(testRevisionData, null, 2)
    );

    const revisionResponse = await fetch(`${API_BASE_URL}/revisions`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(testRevisionData),
    });

    console.log("Revision response status:", revisionResponse.status);

    if (!revisionResponse.ok) {
      const errorText = await revisionResponse.text();
      console.error("Revision creation failed:", errorText);
      return;
    }

    const revisionData = await revisionResponse.json();
    console.log("Revision created successfully:", revisionData);
  } catch (error) {
    console.error("Error:", error.message);
  }
}

testRevisionCreation();
