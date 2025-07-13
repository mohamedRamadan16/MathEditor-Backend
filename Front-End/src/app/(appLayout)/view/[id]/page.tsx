"use client";
import { useEffect, useState } from "react";
import { useSelector, useDispatch, actions } from "@/store";
import { usePathname } from "next/navigation";
import SplashScreen from "@/components/SplashScreen";
import ViewDocument from "@/components/ViewDocument";
import Editor from "@/components/Editor";

const ViewDocumentPage = () => {
  const [fullDocument, setFullDocument] = useState<any>(null);
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

          // Check if document is private and user doesn't have access
          if (documentData.private) {
            if (!user) {
              setError({
                title: "Access Denied",
                subtitle: "This document is private. Please log in to view it.",
              });
              setLoading(false);
              return;
            }

            const isAuthor = documentData.author?.id === user.id;
            const isCoauthor = documentData.coauthors?.some(
              (c: any) => c.id === user.id
            );

            if (!isAuthor && !isCoauthor) {
              setError({
                title: "Access Denied",
                subtitle:
                  "This document is private and you don't have permission to view it",
              });
              setLoading(false);
              return;
            }
          }

          setFullDocument(documentData);
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
  }, [id, dispatch, user]);

  if (loading) return <SplashScreen title="Loading Document" />;
  if (error)
    return <SplashScreen title={error.title} subtitle={error.subtitle} />;
  if (!fullDocument) return <SplashScreen title="Document Not Found" />;

  // Extract cloud document info and editor document data
  const { data, latestRevision, ...cloudDocument } = fullDocument;
  const editorDocument = { data, ...fullDocument };

  return (
    <div>
      <title>{fullDocument.name}</title>

      {/* Read-only mode banner */}
      <div
        style={{
          backgroundColor: "#f5f5f5",
          padding: "8px 16px",
          borderBottom: "1px solid #ddd",
          textAlign: "center",
          fontSize: "14px",
          color: "#666",
        }}
      >
        📖 Viewing document in read-only mode
      </div>

      <ViewDocument cloudDocument={cloudDocument} user={user || undefined}>
        {/* Read-only editor */}
        <Editor document={editorDocument} readOnly={true} />
      </ViewDocument>
    </div>
  );
};

export default ViewDocumentPage;
