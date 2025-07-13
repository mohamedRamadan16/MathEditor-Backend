const API_BASE_URL = "http://localhost:5041/api";

async function testGetDocumentByHandle() {
  try {
    console.log("Testing document lookup by handle: Hello-From-Hell");

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

    // List all documents first
    console.log("\n=== All Documents ===");
    const documentsResponse = await fetch(
      `${API_BASE_URL}/documents?page=1&pageSize=10`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    if (documentsResponse.ok) {
      const documentsData = await documentsResponse.json();
      console.log("Documents found:", documentsData.data?.items?.length || 0);
      documentsData.data?.items?.forEach((doc) => {
        console.log(`- ${doc.handle} (${doc.name}) - ID: ${doc.id}`);
      });
    } else {
      console.log("Failed to fetch documents:", documentsResponse.status);
    }

    // Try to fetch document by handle "Hello-From-Hell"
    console.log('\n=== Trying to fetch "Hello-From-Hell" ===');
    const docResponse = await fetch(
      `${API_BASE_URL}/documents/Hello-From-Hell`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    console.log("Document fetch response status:", docResponse.status);

    if (!docResponse.ok) {
      const errorText = await docResponse.text();
      console.log("Document fetch failed:", errorText);
    } else {
      const docData = await docResponse.json();
      console.log("Document found:", docData);
    }
  } catch (error) {
    console.error("Error:", error.message);
  }
}

testGetDocumentByHandle();
