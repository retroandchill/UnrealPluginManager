import {Avatar, Box, Button, Chip, Menu, MenuItem} from "@mui/material";
import {useAuth} from "react-oidc-context";
import {MouseEvent, useState} from "react";
import {useActiveUserQuery} from "@/queries";

export function UserSigninStatus() {
  const auth = useAuth();
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const open = Boolean(anchorEl);

  const currentUser = useActiveUserQuery({
    enabled: auth.isAuthenticated,
  });

  function handleLogin() {
    auth.signinRedirect();
  }

  function handleLogout() {
    auth.signoutRedirect();
  }

  function handleMenuOpen(event: MouseEvent<HTMLElement>) {
    setAnchorEl(event.currentTarget);
  }

  function handleMenuClose() {
    setAnchorEl(null);
  }

  // If the user isn't logged in or authentication is still in progress
  if (!auth.isAuthenticated) {
    return (
        <Button variant="contained" color="primary" onClick={handleLogin}>
          Login
        </Button>
    );
  }

  // User is authenticated
  return (
      <Box display="flex" alignItems="center">
        <Chip
            avatar={<Avatar>{currentUser.data?.username[0].toUpperCase() ?? "U"}</Avatar>}
            label={currentUser.data?.username ?? "User"}
            color="primary"
            onClick={handleMenuOpen}
            sx={{cursor: "pointer"}}
        />
        <Menu
            anchorEl={anchorEl}
            open={open}
            onClose={handleMenuClose}
            anchorOrigin={{
              vertical: "bottom",
              horizontal: "right",
            }}
            transformOrigin={{
              vertical: "top",
              horizontal: "right",
            }}
        >
          <MenuItem onClick={handleLogout}>Logout</MenuItem>
        </Menu>
      </Box>
  );
}