import { ParagraphNode } from "lexical";
import { $findMatchingParent } from "@lexical/utils";
import { Bookmark, BookmarkEnd, BookmarkStart, bookmarkUniqueNumericIdGen, convertInchesToTwip, ImageRun, IParagraphOptions, Paragraph, Table, TableBorders, TableCell, TableRow, TextRun, TextWrappingType } from "docx";
import { $convertEditortoDocx } from ".";
import sizeOf from 'image-size';
import { $getNodeStyleValueForProperty } from "@/editor/nodes/utils";
import { $isLayoutContainerNode, $isLayoutItemNode } from "@/editor/nodes/LayoutNode";
import { ImageNode } from "@/editor/nodes/ImageNode";

export function $convertImageNode(node: ImageNode) {
  const dataURI = node.getSrc();
  const type = dataURI.split(",")[0].split(";")[0].split("/")[1].split("+")[0] as any;
  const src = dataURI.split(",")[1];
  const data = type === 'svg' ? svgToBuffer(src) : Buffer.from(src, 'base64');
  const altText = node.getAltText();
  const dimensions = sizeOf(data);
  const width = node.getWidth() || dimensions.width as number;
  const height = node.getHeight() || dimensions.height as number;
  const aspect = height / width;
  const float = $getNodeStyleValueForProperty(node, 'float');
  const nearesttLayoutContainer = $findMatchingParent(node, $isLayoutContainerNode);
  const layoutTemplate = nearesttLayoutContainer?.getTemplateColumns().split(' ').map(parseFloat);
  const LayoutItemNodeIndex = $findMatchingParent(node, $isLayoutItemNode)?.getIndexWithinParent();
  const maxLayoutItemWidth = layoutTemplate ? 600 * layoutTemplate[LayoutItemNodeIndex || 0] / layoutTemplate.reduce((a, b) => a + b, 0) : 600;
  const maxWidth = float ? maxLayoutItemWidth / 2 : maxLayoutItemWidth;
  const newWidth = Math.min(width, maxWidth);
  const newHeight = newWidth * aspect;
  const showCaption = node.getShowCaption();

  const imageRun = new ImageRun({
    type,
    data,
    altText: { title: altText, description: altText, name: altText },
    transformation: { width: newWidth, height: newHeight, },
    fallback: { type: 'png', data, },
    floating: float && !showCaption ? {
      horizontalPosition: { relative: 'margin', align: float === 'left' ? 'left' : 'right' },
      verticalPosition: { relative: 'paragraph', align: 'top' },
      allowOverlap: false,
      wrap: { type: TextWrappingType.SQUARE, side: float === 'left' ? 'right' : 'left' },
      margins: { top: 0, bottom: 0, left: float === 'right' ? 100720 : 0, right: float === 'left' ? 100720 : 0 },
    } : undefined,
  });

  const caption = node.__caption;
  const captionChildren = showCaption ? caption.getEditorState().read($convertEditortoDocx) : [];
  const id = node.getId();

  if (!showCaption && !id) return [imageRun];
  const linkId = bookmarkUniqueNumericIdGen()();
  if (!showCaption) return [new BookmarkStart(id, linkId), imageRun, new BookmarkEnd(linkId), new TextRun({ text: '', break: 1, vanish: !showCaption }), ...captionChildren];
  const parent = node.getParent() as ParagraphNode;
  const alignment = parent.getFormatType().replace('justify', 'both') as IParagraphOptions['alignment'];
  const indent = parent.getIndent();

  return new Table({
    rows: [
      new TableRow({
        children: [
          new TableCell({
            children: [
              new Paragraph({
                children: [id ? new Bookmark({ id, children: [imageRun] }) : imageRun],
                alignment, indent: { left: convertInchesToTwip(indent / 2) },
              })],
          }),
        ],
      }),
      new TableRow({
        children: [
          new TableCell({
            children: captionChildren,
          })
        ],
      }),
    ],
    borders: TableBorders.NONE,
    layout: 'fixed',
    columnWidths: [newWidth * 15],
    alignment: alignment,
    width: (float || alignment !== 'both') ? { size: newWidth * 15, type: 'dxa' } : { size: 100, type: 'pct', },
    float: float ? {
      horizontalAnchor: 'text',
      verticalAnchor: 'text',
      relativeHorizontalPosition: float === 'left' ? 'left' : 'right',
      relativeVerticalPosition: 'bottom',
      overlap: 'never',
      leftFromText: float === 'right' ? 16 * 15 : 0,
      rightFromText: float === 'left' ? 16 * 15 : 0,
      topFromText: 0,
      bottomFromText: 0,
    } : undefined,
  });
}

function svgToBuffer(svg: string) {
  const html = decodeURIComponent(svg).replace(/<!-- payload-start -->\s*(.+?)\s*<!-- payload-end -->/, "").replaceAll("//dist", "");
  return Buffer.from(html);
}