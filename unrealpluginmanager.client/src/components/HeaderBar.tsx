import {Logo} from "./icons";
import {SearchBar} from "./common";
import {AppBar, Box, Container, Link, Toolbar} from "@mui/material";
import {useNavigate} from "react-router";
import {UserSigninStatus} from "@/components/info";

export function HeaderBar() {
  const navigate = useNavigate()
  
  
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
        <UserSigninStatus/>
      </Toolbar>
    </Container>
  </AppBar>;
}