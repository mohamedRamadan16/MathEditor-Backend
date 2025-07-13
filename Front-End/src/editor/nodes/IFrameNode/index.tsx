/**
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

import type {
  DOMConversionMap,
  DOMConversionOutput,
  DOMExportOutput,
  LexicalEditor,
  LexicalNode,
  NodeKey,
  Spread,
} from 'lexical';

import { JSX } from "react";
import { ImageNode, ImagePayload, SerializedImageNode } from '../ImageNode';
import { $generateHtmlFromNodes } from "@lexical/html";
import ImageComponent from '../ImageNode/ImageComponent';

function convertIFrameElement(domNode: HTMLElement,): null | DOMConversionOutput {
  const src = domNode.getAttribute('data-lexical-iFrame');
  if (src) {
    const width = +(domNode.getAttribute('width') || '560');
    const height = +(domNode.getAttribute('height') || '315');
    const style = domNode.style.cssText;
    const altText = domNode.title;
    const id = domNode.id;
    const node = $createIFrameNode({ src, width, height, style, id, altText });
    return { node };
  }
  return null;
}

export type IFramePayload = ImagePayload;
export type SerializedIFrameNode = Spread<
  {
    type: 'iframe';
    version: 1;
  },
  SerializedImageNode
>;

export class IFrameNode extends ImageNode {

  static getType(): string {
    return 'iframe';
  }

  static clone(node: IFrameNode): IFrameNode {
    return new IFrameNode(
      node.__src,
      node.__altText,
      node.__width,
      node.__height,
      node.__style,
      node.__id,
      node.__showCaption,
      node.__caption,
      node.__key,
    );

  }

  static importJSON(serializedNode: SerializedIFrameNode): IFrameNode {
    const { width, height, src, style, id, showCaption, caption, altText } =
      serializedNode;
    const node = $createIFrameNode({
      src,
      width,
      height,
      style,
      id,
      showCaption,
      altText
    });
    try {
      if (caption) {
        const nestedEditor = node.__caption;
        const editorState = nestedEditor.parseEditorState(caption.editorState);
        if (!editorState.isEmpty()) {
          nestedEditor.setEditorState(editorState);
        }
      }
    } catch (e) { console.error(e); }
    return node;
  }

  exportJSON(): SerializedIFrameNode {
    return {
      ...super.exportJSON(),
      type: 'iframe',
      version: 1,
    };
  }

  constructor(
    src: string,
    altText: string,
    width: number,
    height: number,
    style: string,
    id: string,
    showCaption?: boolean,
    caption?: LexicalEditor,
    key?: NodeKey,
  ) {
    super(src, altText, width, height, style, id, showCaption, caption, key);
  }

  exportDOM(editor: LexicalEditor): DOMExportOutput {
    const element = super.createDOM(editor._config, editor);
    if (!element) return { element };
    const iframe = document.createElement('iframe');
    iframe.setAttribute('data-lexical-iFrame', this.__src);
    if (this.__width) iframe.setAttribute('width', this.__width.toString());
    if (this.__height) iframe.setAttribute('height', this.__height.toString());
    const matchYoutube = /^.*(youtu\.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/.exec(this.__src);
    const videoId = matchYoutube ? (matchYoutube?.[2].length === 11 ? matchYoutube[2] : null) : null;
    iframe.setAttribute('src', videoId ? `https://www.youtube-nocookie.com/embed/${videoId}` : this.__src);
    iframe.setAttribute('frameborder', '0');
    iframe.setAttribute(
      'allow',
      'accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture',
    );
    iframe.setAttribute('allowfullscreen', 'true');
    iframe.setAttribute('title', this.__altText);
    element.appendChild(iframe);
    if (!this.__showCaption) return { element };
    const caption = document.createElement('figcaption');
    this.__caption.getEditorState().read(() => {
      caption.innerHTML = $generateHtmlFromNodes(this.__caption);
    });
    element.appendChild(caption);
    return { element };
  }

  static importDOM(): DOMConversionMap | null {
    return {
      iframe: (domNode: HTMLElement) => {
        if (!domNode.hasAttribute('data-lexical-iFrame')) {
          return null;
        }
        return {
          conversion: convertIFrameElement,
          priority: 1,
        };
      },
    };
  }

  getTextContent(
    _includeInert?: boolean | undefined,
    _includeDirectionless?: false | undefined,
  ): string {
    return this.__src;
  }

  decorate(): JSX.Element {
    const self = this.getLatest();
    const matchYoutube = /^.*(youtu\.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/.exec(self.__src);
    const videoId = matchYoutube ? (matchYoutube?.[2].length === 11 ? matchYoutube[2] : null) : null;
    const src = videoId ? `https://www.youtube-nocookie.com/embed/${videoId}` : self.__src;

    return (
      <ImageComponent
        src={src}
        altText={self.__altText}
        width={self.__width}
        height={self.__height}
        nodeKey={self.__key}
        showCaption={self.__showCaption}
        caption={self.__caption}
        element='iframe'
      />
    );
  }
}

export function $createIFrameNode(payload: IFramePayload): IFrameNode {
  const { src, altText = "iframe", width, height, style, id, showCaption, caption, key } = payload;
  return new IFrameNode(src, altText, width, height, style, id, showCaption, caption, key);
}

export function $isIFrameNode(
  node: IFrameNode | LexicalNode | null | undefined,
): node is IFrameNode {
  return node instanceof IFrameNode;
}