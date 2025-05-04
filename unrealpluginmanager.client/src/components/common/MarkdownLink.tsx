import Link from "@mui/material/Link";
import {AnchorHTMLAttributes, ClassAttributes} from "react";
import {ExtraProps} from "react-markdown";

type MarkdownLinkProps = ClassAttributes<HTMLAnchorElement> & AnchorHTMLAttributes<HTMLAnchorElement> & ExtraProps;

export function MarkdownLink({node, href, ...props}: MarkdownLinkProps) {
  return <Link
      href={href}
      color="primary"
      underline="hover"
      {...props}
  />;
}