import { $isTableCellNode } from "@/editor/nodes/TableNode";
import { $findMatchingParent } from "@lexical/utils";
import { $isListItemNode } from "@lexical/list";
import { $isLinkNode } from "@lexical/link";
import { $getNodeStyleValueForProperty } from "@/editor/nodes/utils";
import { TextRun } from "docx";
import { ElementNode, TextNode } from "lexical";

export function $convertTextNode(node: TextNode) {
  const textContent = node.getTextContent();
  const nearestListItem = $findMatchingParent(node, $isListItemNode);
  const isCheckedText = nearestListItem?.getChecked();
  const isLinkText = $findMatchingParent(node, $isLinkNode);
  const nearestTableCell = $findMatchingParent(node, $isTableCellNode);
  const tableCellColor = nearestTableCell ? $getNodeStyleValueForProperty(nearestTableCell, 'color').replace('inherit', '') : undefined;
  const fontsizeInPx = parseInt($getNodeStyleValueForProperty(node, 'font-size'));
  const color = $getNodeStyleValueForProperty(node, 'color').replace('inherit', '') || tableCellColor;
  const backgroundColor = $getNodeStyleValueForProperty(node, 'background-color').replace('inherit', '') || undefined;
  const parent = node.getParent<ElementNode>();
  const direction = parent?.getDirection();

  const textRun = new TextRun({
    text: textContent,
    bold: node.hasFormat('bold') || undefined,
    italics: node.hasFormat('italic'),
    strike: node.hasFormat('strikethrough') || isCheckedText,
    underline: node.hasFormat('underline') ? { type: "single" } : undefined,
    color: color,
    highlight: node.hasFormat('highlight') ? 'yellow' : undefined,
    subScript: node.hasFormat('subscript'),
    superScript: node.hasFormat('superscript'),
    font: node.hasFormat('code') ? 'Consolas' : $getNodeStyleValueForProperty(node, 'font-family'),
    size: fontsizeInPx ? fontsizeInPx * 1.5 : undefined,
    shading: backgroundColor || node.hasFormat('code') ? ({
      fill: node.hasFormat('code') ? '#F2F4F6' : backgroundColor,
    }) : undefined,
    style: isLinkText ? 'Hyperlink' : undefined,
    rightToLeft: direction === 'rtl',
  });

  return textRun;

}