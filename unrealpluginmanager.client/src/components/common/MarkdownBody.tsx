import {Typography} from "@mui/material";
import {ClassAttributes, HTMLAttributes} from "react";
import {ExtraProps} from "react-markdown";

type MarkdownBodyProps = ClassAttributes<HTMLParagraphElement> & HTMLAttributes<HTMLParagraphElement> & ExtraProps;

export function MarkdownBody({node, ...props}: MarkdownBodyProps) {
  return <Typography variant="body1" component="p" {...props} />;
}