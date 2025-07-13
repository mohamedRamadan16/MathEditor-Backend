"use client"
import dynamic from 'next/dynamic';
import { useState, useEffect } from 'react';
import { EditorSkeleton } from '../EditorSkeleton';
import SplashScreen from '../SplashScreen';

const Playground: React.FC<React.PropsWithChildren> = ({ children }) => {
  const [isClient, setIsClient] = useState(false)
  useEffect(() => { setIsClient(true) }, [])
  const fallback = children ? <EditorSkeleton>{children}</EditorSkeleton> : <SplashScreen title="Loading Document" />;
  if (!isClient) return fallback;

  const PlaygroundEditor = dynamic(() => import('./Editor'), { ssr: false, loading: () => fallback });
  return (
    <PlaygroundEditor>{children}</PlaygroundEditor>
  );
}

export default Playground;