const API_BASE_URL = "http://localhost:5041/api";

// Test data with problematic format values (integers instead of strings)
const testRevisionData = {
  documentId: "c025a715-6953-4fef-9fe6-9b959d797ede",
  data: {
    root: {
      children: [
        {
          children: [
            {
              detail: 0,
              format: 0, // Integer - this should be converted to string
              mode: "normal",
              style: "",
              text: "Hello World Fixed",
              type: "text",
              version: 1,
            },
          ],
          direction: "ltr",
          format: 0, // Integer - this should be converted to string
          indent: 0,
          type: "paragraph",
          version: 1,
        },
      ],
      direction: "ltr",
      format: 0, // Integer - this should be converted to string
      indent: 0,
      type: "root",
      version: 1,
    },
  },
};

// Inline the conversion function
function convertSerializedEditorStateToLexicalState(state) {
  // Helper function to convert a node recursively
  const convertNode = (node) => {
    if (!node) return node;

    const converted = {
      children: node.children ? node.children.map(convertNode) : null,
      direction: node.direction || null,
      format: String(node.format || ""), // Ensure format is always a string
      indent: node.indent || 0,
      type: node.type || "",
      version: node.version || 1,
      // Text node specific properties
      text: node.text || null,
      detail: node.detail || null,
      mode: node.mode || null,
      style: node.style || null,
    };

    // Remove null values to match DTO structure
    Object.keys(converted).forEach((key) => {
      if (converted[key] === null) {
        delete converted[key];
      }
    });

    return converted;
  };

  return {
    root: {
      children: state.root.children ? state.root.children.map(convertNode) : [],
      direction: state.root.direction || "ltr",
      format: String(state.root.format || ""), // Ensure format is always a string
      indent: state.root.indent || 0,
      type: state.root.type || "root",
      version: state.root.version || 1,
    },
  };
}

async function testRevisionCreationFixed() {
  try {
    console.log("Testing revision creation with problematic data...");

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
    const token = loginData.token;

    console.log("Original data (problematic):");
    console.log(JSON.stringify(testRevisionData, null, 2));

    // Convert the data using our fixed function
    const convertedData = {
      documentId: testRevisionData.documentId,
      data: convertSerializedEditorStateToLexicalState(testRevisionData.data),
    };

    console.log("\nConverted data (fixed):");
    console.log(JSON.stringify(convertedData, null, 2));

    // Now try to create a revision
    const revisionResponse = await fetch(`${API_BASE_URL}/revisions`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(convertedData),
    });

    console.log("\nRevision response status:", revisionResponse.status);

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

testRevisionCreationFixed();
