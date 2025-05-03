import {Logo} from "./icons";
import {SearchBar} from "./common";
import {AppBar, Box, Button, Container, Link, Toolbar} from "@mui/material";

interface HeaderBarProps {
  onSearch: (search: string) => void;
}

export function HeaderBar({onSearch}: HeaderBarProps) {
  return <AppBar position="static">
    <Container>
      <Toolbar style={{marginTop: "10px", marginBottom: "10px"}}>
        <Box display="flex" flexGrow={1} alignItems="center">
          <Link href="/" underline="none" sx={{display: "flex", alignItems: "center"}}>
            <Logo/>
          </Link>
          <SearchBar sx={{marginX: "10px"}}
                     onSearch={onSearch}/>
        </Box>
        <Button variant="contained" color="primary">Login</Button>
      </Toolbar>
    </Container>
  </AppBar>;
}