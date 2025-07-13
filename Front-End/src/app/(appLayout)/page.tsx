import Home from "@/components/Home";
import type { Metadata } from "next";
import { ThumbnailProvider } from "@/app/context/ThumbnailContext";

export const metadata: Metadata = {
  title: "Math Editor",
  description:
    "Math Editor is a free text editor, with support for LaTeX, Geogebra, Excalidraw and markdown shortcuts. Create, share and print math documents with ease.",
};

const page = async () => {
  // All serverless/Prisma logic removed. Replace with static or API-fetched data as needed.
  return (
    <ThumbnailProvider thumbnails={{}}>
      <Home staticDocuments={[]} />
    </ThumbnailProvider>
  );
};

export default page;
