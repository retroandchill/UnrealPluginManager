import {Logo} from "./icons";
import {SearchBar} from "./common";
import {AppBar, Box, Button, Container, Link, Toolbar} from "@mui/material";
import {useNavigate} from "react-router";

export function HeaderBar() {
  const navigate = useNavigate();
  
  return <AppBar position="static">
    <Container>
      <Toolbar style={{marginTop: "10px", marginBottom: "10px"}}>
        <Box display="flex" flexGrow={1} alignItems="center">
          <Link href="/" underline="none" sx={{display: "flex", alignItems: "center"}}>
            <Logo/>
          </Link>
          <SearchBar sx={{marginX: "10px"}}
                     onSearch={search => navigate(`/search?q=${search}`)}/>
        </Box>
        <Button variant="contained" color="primary">Login</Button>
      </Toolbar>
    </Container>
  </AppBar>;
}