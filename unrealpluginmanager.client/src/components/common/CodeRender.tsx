import {ClassAttributes, HTMLAttributes} from "react";
import {ExtraProps} from "react-markdown";
import {Prism as SyntaxHighlighter} from 'react-syntax-highlighter';
import {dracula} from "react-syntax-highlighter/dist/cjs/styles/prism";

type CodeRenderProps = ClassAttributes<HTMLElement> & HTMLAttributes<HTMLElement> & ExtraProps;

/**
 * Renders code with syntax highlighting when a language is detected in the className.
 * Falls back to a styled inline code block when no language is detected.
 *
 * @param options - The options for rendering the component.
 * @param options.node - The node to be rendered (unused in this function).
 * @param options.className - The class name that may contain the language specifier.
 * @param options.children - The code content to be rendered.
 * @param options.style - Inline styles to be applied to the rendered component.
 * @param options.ref - The reference to the DOM element or component.
 * @param options.props - Additional properties to be passed to the rendered component.
 * @return The rendered syntax-highlighted code block or fallback styled inline code.
 */
export function CodeRender({node, className, children, style, ref, ...props}: CodeRenderProps) {
  const match = /language-(\w+)/.exec(className || "");
  return match ? (
      <SyntaxHighlighter
          style={dracula}
          language={match[1]}
          PreTag="div"
          {...props}
      >
        {String(children).trim()}
      </SyntaxHighlighter>
  ) : (
      <code
          style={{
            backgroundColor: "#282a36",
            padding: "0.2em 0.4em",
            borderRadius: "4px",
            fontSize: "0.95rem",
          }}
          {...props}
      >
        {children}
      </code>
  );
}