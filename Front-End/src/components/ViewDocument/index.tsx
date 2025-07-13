"use client";
import { useEffect, useState } from "react";
import { useSelector, useDispatch, actions } from "@/store";
import { usePathname } from "next/navigation";
import SplashScreen from "@/components/SplashScreen";

const ViewDocument: React.FC = () => {
  const [document, setDocument] = useState<any>(null);
  const [error, setError] = useState<{
    title: string;
    subtitle?: string;
  } | null>(null);
  const [loading, setLoading] = useState(true);

  const dispatch = useDispatch();
  const pathname = usePathname();
  const user = useSelector((state) => state.user);

  const id = pathname.split("/")[2]?.toLowerCase();

  useEffect(() => {
    const loadDocument = async () => {
      if (!id) {
        setError({
          title: "Document Not Found",
          subtitle: "No document ID provided",
        });
        setLoading(false);
        return;
      }

      try {
        console.log("Fetching document with ID:", id);
        const response = await dispatch(actions.getCloudDocument(id));
        console.log("Document response:", response);

        if (response.type === actions.getCloudDocument.fulfilled.type) {
          const documentData = response.payload as any;
          console.log("Document data received:", documentData);
          setDocument(documentData);
        } else {
          console.log("Document fetch failed:", response.payload);
          setError({
            title: "Document Not Found",
            subtitle:
              "The document you're looking for doesn't exist or has been removed",
          });
        }
      } catch (err) {
        console.error("Error loading document:", err);
        setError({
          title: "Error Loading Document",
          subtitle: "An error occurred while loading the document",
        });
      }

      setLoading(false);
    };

    loadDocument();
  }, [id, dispatch]);

  if (loading) return <SplashScreen title="Loading Document" />;
  if (error)
    return <SplashScreen title={error.title} subtitle={error.subtitle} />;
  if (!document) return <SplashScreen title="Document Not Found" />;

  return (
    <div>
      <title>{document.name || "Document"}</title>

      {/* Debug information */}
      <div
        style={{
          backgroundColor: "#f0f0f0",
          padding: "16px",
          margin: "16px",
          borderRadius: "8px",
          fontFamily: "monospace",
          fontSize: "12px",
        }}
      >
        <h3>Debug Information:</h3>
        <p>
          <strong>Document ID:</strong> {document.id}
        </p>
        <p>
          <strong>Document Handle:</strong> {document.handle || "MISSING"}
        </p>
        <p>
          <strong>Document Name:</strong> {document.name}
        </p>
        <p>
          <strong>Author:</strong>{" "}
          {document.author
            ? `${document.author.name} (${document.author.handle})`
            : "MISSING"}
        </p>
        <p>
          <strong>Private:</strong> {document.private ? "Yes" : "No"}
        </p>
        <p>
          <strong>User ID:</strong> {user?.id || "Not logged in"}
        </p>
        <p>
          <strong>Has Data:</strong> {document.data ? "Yes" : "No"}
        </p>
      </div>

      {/* Simple message for now */}
      <div style={{ padding: "16px", textAlign: "center" }}>
        <h1>Document: {document.name}</h1>
        <p>Successfully loaded document data!</p>
        <p>Document structure is working correctly.</p>
      </div>
    </div>
  );
};

export default ViewDocument;
