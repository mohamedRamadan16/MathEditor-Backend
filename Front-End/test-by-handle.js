const API_BASE_URL = "http://localhost:5041/api";

async function testGetByHandle() {
  try {
    console.log("Testing document fetch using the correct by-handle endpoint");

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

    // Try to fetch document by handle using the correct endpoint
    console.log("\n=== Trying to fetch using by-handle endpoint ===");
    const docResponse = await fetch(
      `${API_BASE_URL}/documents/by-handle/Hello-From-Hell`,
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
      console.log("Document found successfully!");
      console.log("Document data:", JSON.stringify(docData, null, 2));

      // Now try to fetch the revision
      if (docData.data && docData.data.head) {
        console.log("\n=== Fetching latest revision ===");
        const revResponse = await fetch(
          `${API_BASE_URL}/revisions/${docData.data.head}`,
          {
            method: "GET",
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (revResponse.ok) {
          const revData = await revResponse.json();
          console.log("Revision data:", JSON.stringify(revData, null, 2));
        } else {
          console.log("Failed to fetch revision:", revResponse.status);
        }
      }
    }
  } catch (error) {
    console.error("Error:", error.message);
  }
}

testGetByHandle();
