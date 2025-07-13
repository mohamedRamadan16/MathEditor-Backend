import type { Metadata } from "next";
import { Suspense } from "react";
import UserCard from "@/components/User/UserCard";
import UserDocuments from "@/components/User/UserDocuments";
import { ThumbnailProvider } from "@/app/context/ThumbnailContext";
import { sortDocuments } from "@/components/DocumentControls/sortDocuments";
import { CloudDocument } from "@/types";

export async function generateMetadata(props: {
  params: Promise<{ id: string }>;
}): Promise<Metadata> {
  // All serverless/Prisma logic removed. Replace with static or API-fetched data as needed.
  return {
    title: "Math Editor",
    description: "User profile page",
    openGraph: {
      images: [],
    },
  };
}

const UserCardWrapper = async ({ id }: { id: string }) => {
  // All serverless/Prisma logic removed. Replace with static or API-fetched data as needed.
  return <UserCard user={undefined} />;
};

const UserDocumentsWrapper = async ({
  id,
  page,
  sortKey,
  sortDirection,
}: {
  id: string;
  page: string;
  sortKey: string;
  sortDirection: "asc" | "desc";
}) => {
  // All serverless/Prisma logic removed. Replace with static or API-fetched data as needed.
  return (
    <ThumbnailProvider thumbnails={{}}>
      <UserDocuments documents={[]} pages={1} />
    </ThumbnailProvider>
  );
};

export default async function Page(props: {
  params: Promise<{ id: string }>;
  searchParams: Promise<{
    page?: string;
    sortKey?: string;
    sortDirection?: "asc" | "desc";
  }>;
}) {
  const params = await props.params;
  const searchParams = await props.searchParams;
  return (
    <>
      <Suspense fallback={<UserCard />}>
        <UserCardWrapper id={params.id} />
      </Suspense>
      <Suspense fallback={<UserDocuments />}>
        <UserDocumentsWrapper
          id={params.id}
          page={searchParams.page || "1"}
          sortKey={searchParams.sortKey || "updatedAt"}
          sortDirection={searchParams.sortDirection || "desc"}
        />
      </Suspense>
    </>
  );
}
