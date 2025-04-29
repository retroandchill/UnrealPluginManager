import {Button, Card, CardContent, Divider, Link, List, ListItem, ListItemIcon, ListItemText} from '@mui/material';
import CalendarTodayOutlinedIcon from '@mui/icons-material/CalendarTodayOutlined';
import GetAppOutlinedIcon from '@mui/icons-material/GetAppOutlined';
import UpdateOutlinedIcon from '@mui/icons-material/UpdateOutlined';
import PersonOutlineOutlinedIcon from '@mui/icons-material/PersonOutlineOutlined';
import LinkOutlinedIcon from '@mui/icons-material/LinkOutlined';
import GppBadOutlinedIcon from '@mui/icons-material/GppBadOutlined';
import {linkStyle} from "@/Theme.ts";
import {PluginVersionInfo} from "@/api";

interface PluginPageSidebarProps {
  plugin: PluginVersionInfo;
}

export function PluginPageSidebar({plugin}: PluginPageSidebarProps) {
  return (
      <Card elevation={3}>
        <CardContent>
          <List sx={{'& .MuiListItem-root': {px: 0}}}>
            <ListItem>
              <ListItemIcon>
                <GetAppOutlinedIcon/>
              </ListItemIcon>
              <ListItemText
                  primary="Weekly Downloads"
                  secondary="1,234"
              />
            </ListItem>
            <Divider/>

            <ListItem>
              <ListItemIcon>
                <UpdateOutlinedIcon/>
              </ListItemIcon>
              <ListItemText
                  primary="Latest Version"
                  secondary={plugin.version}
              />
            </ListItem>
            <Divider/>

            <ListItem>
              <ListItemIcon>
                <CalendarTodayOutlinedIcon/>
              </ListItemIcon>
              <ListItemText
                  primary="Last Updated"
                  secondary="2 days ago"
              />
            </ListItem>
            <Divider/>

            <ListItem>
              <ListItemIcon>
                <PersonOutlineOutlinedIcon/>
              </ListItemIcon>
              <ListItemText
                  primary="Author"
                  secondary={plugin.authorName || "Unknown"}
              />
            </ListItem>
            <Divider/>

            <ListItem>
              <ListItemIcon>
                <LinkOutlinedIcon/>
              </ListItemIcon>
              <ListItemText
                  primary="Homepage"
                  secondary={plugin.authorWebsite ?
                      <Link href={plugin.authorWebsite}
                            color="inherit"
                            sx={linkStyle}
                      >
                        {plugin.authorWebsite}
                      </Link> :
                      "Not specified"}

              />
            </ListItem>
            <Divider/>

            <ListItem
                sx={{
                  mt: 2,
                  '& .MuiButton-root': {
                    width: '100%',
                    color: 'white',
                    backgroundColor: 'error.main',
                    '&:hover': {
                      backgroundColor: 'error.dark',
                    }
                  }
                }}
            >
              <Button
                  variant="contained"
                  startIcon={<GppBadOutlinedIcon/>}>
                Report as Malware
              </Button>
            </ListItem>
          </List>
        </CardContent>
      </Card>
  );
}