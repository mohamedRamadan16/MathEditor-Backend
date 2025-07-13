/**
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

import {
  DOMExportOutput,
  isHTMLElement,
  LexicalEditor,
  LexicalNode,
  NodeKey,
  Spread,
} from "lexical";
import {
  NonDeleted,
  ExcalidrawElement,
} from "@excalidraw/excalidraw/element/types";

import { ImageNode, ImagePayload, SerializedImageNode } from "../ImageNode";
import { $generateHtmlFromNodes } from "@lexical/html";

import ImageComponent from "../ImageNode/ImageComponent";
import htmr from "htmr";
import { JSX } from "react";

export type SketchPayload = Spread<
  {
    /**
     * @deprecated The value is now embedded in the src
     */
    value?: NonDeleted<ExcalidrawElement>[];
  },
  ImagePayload
>;

export type SerializedSketchNode = Spread<
  {
    value?: NonDeleted<ExcalidrawElement>[];
    type: "sketch";
    version: 1;
  },
  SerializedImageNode
>;

export class SketchNode extends ImageNode {
  __value?: NonDeleted<ExcalidrawElement>[];

  static getType(): string {
    return "sketch";
  }

  static clone(node: SketchNode): SketchNode {
    return new SketchNode(
      node.__src,
      node.__altText,
      node.__width,
      node.__height,
      node.__style,
      node.__id,
      node.__value,
      node.__showCaption,
      node.__caption,
      node.__key
    );
  }

  static importJSON(serializedNode: SerializedSketchNode): SketchNode {
    const {
      width,
      height,
      src,
      value,
      style,
      id,
      showCaption,
      caption,
      altText,
    } = serializedNode;

    // Provide default values to prevent undefined errors
    const node = $createSketchNode({
      src: src || "",
      value: value || [],
      width: width || 100,
      height: height || 100,
      style: style || "",
      id: id || "",
      showCaption: showCaption || false,
      altText: altText || "Sketch",
    });

    try {
      if (caption && caption.editorState) {
        const nestedEditor = node.__caption;
        if (nestedEditor) {
          const editorState = nestedEditor.parseEditorState(
            caption.editorState
          );
          if (!editorState.isEmpty()) {
            nestedEditor.setEditorState(editorState);
          }
        }
      }
    } catch (e) {
      console.error("Error importing sketch node caption:", e);
    }
    return node;
  }

  exportDOM(editor: LexicalEditor): DOMExportOutput {
    const element = super.createDOM(editor._config, editor);
    if (element && isHTMLElement(element)) {
      const html = decodeURIComponent(this.__src.split(",")[1]);
      element.innerHTML = html.replace(
        /<!-- payload-start -->\s*(.+?)\s*<!-- payload-end -->/,
        ""
      );
      const svg = element.firstElementChild!;
      const styles = svg.querySelectorAll("style");
      styles.forEach((style) => {
        style.remove();
      });
      if (this.__width) svg.setAttribute("width", this.__width.toString());
      if (this.__height) svg.setAttribute("height", this.__height.toString());
      if (!this.__showCaption) return { element };
      const caption = document.createElement("figcaption");
      this.__caption.getEditorState().read(() => {
        caption.innerHTML = $generateHtmlFromNodes(this.__caption);
      });
      element.appendChild(caption);
    }
    return { element };
  }

  constructor(
    src: string,
    altText: string,
    width: number,
    height: number,
    style: string,
    id: string,
    value?: NonDeleted<ExcalidrawElement>[],
    showCaption?: boolean,
    caption?: LexicalEditor,
    key?: NodeKey
  ) {
    super(src, altText, width, height, style, id, showCaption, caption, key);
    this.__value = value;
  }

  exportJSON(): SerializedSketchNode {
    return {
      ...super.exportJSON(),
      value: this.__value,
      type: "sketch",
      version: 1,
    };
  }

  update(payload: Partial<SketchPayload>): void {
    const writable = this.getWritable();
    super.update(payload);
    writable.__value = payload.value ?? writable.__value;
  }

  getValue(): NonDeleted<ExcalidrawElement>[] | undefined {
    return this.__value;
  }

  decorate(): JSX.Element {
    const self = this.getLatest();

    // Safely handle caption
    let html = "";
    let children = null;
    if (self.__caption) {
      try {
        html = self.__caption
          .getEditorState()
          .read(() => $generateHtmlFromNodes(self.__caption));
        children = htmr(html);
      } catch (e) {
        console.error("Error reading caption:", e);
      }
    }

    // Safely handle src
    let src = self.__src;
    if (self.__src && self.__src.includes(",")) {
      try {
        const decoded = decodeURIComponent(self.__src.split(",")[1])
          .replace(/<!-- payload-start -->\s*(.+?)\s*<!-- payload-end -->/, "")
          .replaceAll("//dist", "");
        src = `data:image/svg+xml,${encodeURIComponent(decoded)}`;
      } catch (e) {
        console.error("Error processing sketch src:", e);
        // Fallback to original src if processing fails
        src = self.__src;
      }
    }

    return (
      <ImageComponent
        width={self.__width}
        height={self.__height}
        src={src}
        altText={self.__altText}
        nodeKey={self.__key}
        showCaption={self.__showCaption}
        caption={self.__caption}
        element="svg"
      >
        {children}
      </ImageComponent>
    );
  }
}

export function $createSketchNode({
  src,
  altText = "Sketch",
  value,
  key,
  width = 0,
  height = 0,
  style = "",
  id = "",
  showCaption = false,
  caption,
}: SketchPayload): SketchNode {
  // Generate a unique ID if not provided
  const nodeId =
    id || `sketch-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;

  return new SketchNode(
    src || "",
    altText,
    width,
    height,
    style,
    nodeId,
    value,
    showCaption,
    caption,
    key
  );
}

export function $isSketchNode(
  node: LexicalNode | null | undefined
): node is SketchNode {
  return node instanceof SketchNode;
}
