import NewDocument from "@/components/NewDocument";
import type { Metadata } from "next";
import SplashScreen from "@/components/SplashScreen";

export async function generateMetadata(): Promise<Metadata> {
  return {
    title: "New Document",
    description: "Create a new document on Math Editor",
    openGraph: {
      images: [],
    },
  };
}

const Page = () => <NewDocument />;

export default Page;
