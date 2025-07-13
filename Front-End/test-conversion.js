// Test the conversion function with problematic data
const testData = {
  root: {
    children: [
      {
        children: [
          {
            detail: 0,
            format: 0, // This is an integer, should be converted to string
            mode: "normal",
            style: "",
            text: "Hello World",
            type: "text",
            version: 1,
          },
        ],
        direction: "ltr",
        format: 0, // This is an integer, should be converted to string
        indent: 0,
        type: "paragraph",
        version: 1,
      },
    ],
    direction: "ltr",
    format: 0, // This is an integer, should be converted to string
    indent: 0,
    type: "root",
    version: 1,
  },
};

// Inline the conversion function for testing
function convertSerializedEditorStateToLexicalState(state) {
  // Helper function to convert a node recursively
  const convertNode = (node) => {
    if (!node) return node;

    const converted = {
      children: node.children ? node.children.map(convertNode) : null,
      direction: node.direction || null,
      format: String(node.format || ""), // Ensure format is always a string
      indent: node.indent || 0,
      type: node.type || "",
      version: node.version || 1,
      // Text node specific properties
      text: node.text || null,
      detail: node.detail || null,
      mode: node.mode || null,
      style: node.style || null,
    };

    // Remove null values to match DTO structure
    Object.keys(converted).forEach((key) => {
      if (converted[key] === null) {
        delete converted[key];
      }
    });

    return converted;
  };

  return {
    root: {
      children: state.root.children ? state.root.children.map(convertNode) : [],
      direction: state.root.direction || "ltr",
      format: String(state.root.format || ""), // Ensure format is always a string
      indent: state.root.indent || 0,
      type: state.root.type || "root",
      version: state.root.version || 1,
    },
  };
}

console.log("Original data:");
console.log(JSON.stringify(testData, null, 2));

console.log("\nConverted data:");
const converted = convertSerializedEditorStateToLexicalState(testData);
console.log(JSON.stringify(converted, null, 2));

console.log("\nData types:");
console.log("Root format type:", typeof converted.root.format);
console.log("Paragraph format type:", typeof converted.root.children[0].format);
console.log(
  "Text format type:",
  typeof converted.root.children[0].children[0].format
);
