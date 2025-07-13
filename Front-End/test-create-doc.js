const API_BASE_URL = "http://localhost:5041/api";

async function createTestDocument() {
  try {
    console.log("Creating test document...");

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
    console.log("Login successful");

    const token = loginData.token;

    // Create a document
    const documentData = {
      handle: "test-document",
      name: "Test Document", // Changed from title to name
      published: false,
      collab: false,
      private: false,
      coauthors: [],
      initialRevision: {
        data: {
          root: {
            children: [
              {
                children: [
                  {
                    detail: 0,
                    format: "",
                    mode: "normal",
                    style: "",
                    text: "Initial content",
                    type: "text",
                    version: 1,
                  },
                ],
                direction: "ltr",
                format: "",
                indent: 0,
                type: "paragraph",
                version: 1,
              },
            ],
            direction: "ltr",
            format: "",
            indent: 0,
            type: "root",
            version: 1,
          },
        },
      },
    };

    const docResponse = await fetch(`${API_BASE_URL}/documents`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(documentData),
    });

    if (!docResponse.ok) {
      const errorText = await docResponse.text();
      console.error("Document creation failed:", errorText);
      return;
    }

    const docData = await docResponse.json();
    console.log("Document created successfully:", docData);

    return docData.data.id;
  } catch (error) {
    console.error("Error:", error.message);
  }
}

createTestDocument();
