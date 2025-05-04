import {SvgIcon} from "@mui/material";
import MainLogo from "@/assets/uepm.svg?react";

export function Logo() {
  return <SvgIcon
      component={MainLogo}
      inheritViewBox
      sx={{
        fontSize: 72,
        marginRight: 2,
      }}/>;
}