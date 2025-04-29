import {pluginsApi} from "@/config";
import Markdown from "react-markdown";
import {Prism as SyntaxHighlighter} from 'react-syntax-highlighter';
import {dracula} from "react-syntax-highlighter/dist/cjs/styles/prism";
import Typography from "@mui/material/Typography";
import remarkGfm from 'remark-gfm'
import {remarkAlert} from 'remark-github-blockquote-alert'
import 'remark-github-blockquote-alert/alert.css'
import {useQuery, useQueryClient} from "@tanstack/react-query";
import {Box, CircularProgress} from "@mui/material";


/**
 * Represents properties required to display the README of a specific plugin version.
 *
 * This interface is used to pass identifiers necessary for fetching and displaying
 * the README file associated with a particular plugin and its version.
 *
 * Properties:
 * - `pluginId`: A string representing the unique identifier of the plugin.
 * - `versionId`: A string representing the unique identifier of the specific version of the plugin.
 */
interface PluginReadmeDisplayProps {
  /**
   * Represents the unique identifier for a plugin.
   * This value is typically used to load, manage, or reference a specific plugin within an application or system.
   * It should be a string that uniquely corresponds to a plugin within the defined context.
   */
  pluginId: string;
  
  /**
   * Represents the unique identifier for a specific version of an entity or object.
   * This identifier is typically used for version tracking and comparison purposes.
   */
  versionId: string;
}

/**
 * Displays the README content of a plugin in Markdown format.
 *
 * @param props - The properties object.
 * @param props.pluginId - The ID of the plugin for which the README is being fetched.
 * @param props.versionId - The specific version ID of the plugin for which the README is being fetched.
 * @return A React component rendering the plugin's README in Markdown format or a loading message if the content is not yet available.
 */
export function PluginReadmeDisplay({pluginId, versionId}: Readonly<PluginReadmeDisplayProps>) {
  const queryClient = useQueryClient();

  const readme = useQuery({
    queryKey: ['plugins', pluginId, versionId, 'readme'],
    queryFn: () => pluginsApi.getPluginReadme({pluginId: pluginId, versionId: versionId}),
  }, queryClient);

  if (readme.isError) {
    return <Typography color="error">Failed to fetch plugin README.</Typography>;
  }

  if (readme.data === undefined) {
    return (
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
          <CircularProgress/>
        </Box>
    );
  }

  return (
      <div className="markdown-container" style={{all: "revert"}}>
        <Markdown remarkPlugins={[remarkGfm, remarkAlert]} components={{
          // Customize how code blocks are rendered
          code: ({node, className, children, style, ref, ...props}) => {
            const match = /language-(\w+)/.exec(className || ""); // Match the language (e.g., language-js)
            return match ? (
                <SyntaxHighlighter
                    style={dracula} // Use your preferred highlighting theme
                    language={match[1]} // Extracted language
                    PreTag="div" // Prevent breaking layout
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
        }}>
          {readme.data}
        </Markdown>
      </div>
  );
}