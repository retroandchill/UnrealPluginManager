import {Box, Container, Divider, Grid, IconButton, Link, Stack, Typography,} from '@mui/material';
import {GitHub} from '@mui/icons-material';
import {linkStyle} from "@/Theme.ts";


export function Footer() {
  const currentYear = new Date().getFullYear();

  const footerSections = {
    support: {
      title: 'Support',
      links: [
        {name: 'Documentation', href: '/docs'},
        {name: 'FAQs', href: '/faqs'},
        {name: 'Community Forums', href: '/community'},
        {name: 'Contact Support', href: '/support'},
      ],
    },
    company: {
      title: 'Organization',
      links: [
        {name: 'About Us', href: '/about'},
        {name: 'Blog', href: '/blog'},
        {name: 'Contact', href: '/contact'},
      ],
    },
    legal: {
      title: 'Terms & Policies',
      links: [
        {name: 'Terms of Service', href: '/terms'},
        {name: 'Privacy Policy', href: '/privacy'},
        {name: 'Cookie Policy', href: '/cookies'},
        {name: 'License Agreement', href: '/license'},
      ],
    },
  };


  return (
      <Box
          component="footer"
          sx={{
            backgroundColor: (theme) => theme.palette.grey[900],
            color: 'white',
            py: 6,
            mt: 'auto',
          }}
      >
        <Container maxWidth="lg">
          <Grid container spacing={5}>
            {/* Support Section */}
            <Grid size={{xs: 12, sm: 6, md: 3}}>
              <Typography variant="h6" gutterBottom>
                {footerSections.support.title}
              </Typography>
              <Stack spacing={1}>
                {footerSections.support.links.map((link) => (
                    <Link
                        key={link.name}
                        href={link.href}
                        color="inherit"
                        sx={linkStyle}
                    >
                      {link.name}
                    </Link>
                ))}
              </Stack>
            </Grid>

            {/* Company Section */}
            <Grid size={{xs: 12, sm: 6, md: 3}}>
              <Typography variant="h6" gutterBottom>
                {footerSections.company.title}
              </Typography>
              <Stack spacing={1}>
                {footerSections.company.links.map((link) => (
                    <Link
                        key={link.name}
                        href={link.href}
                        color="inherit"
                        sx={linkStyle}
                    >
                      {link.name}
                    </Link>
                ))}
              </Stack>
            </Grid>

            {/* Legal Section */}
            <Grid size={{xs: 12, sm: 6, md: 3}}>
              <Typography variant="h6" gutterBottom>
                {footerSections.legal.title}
              </Typography>
              <Stack spacing={1}>
                {footerSections.legal.links.map((link) => (
                    <Link
                        key={link.name}
                        href={link.href}
                        color="inherit"
                        sx={linkStyle}
                    >
                      {link.name}
                    </Link>
                ))}
              </Stack>
            </Grid>

            {/* Social Links Section */}
            <Grid size={{xs: 12, sm: 6, md: 3}}>
              <Typography variant="h6" gutterBottom>
                Connect With Us
              </Typography>
              <Box sx={{mt: 2}}>
                <IconButton
                    href="https://github.com/retroandchill/UnrealPluginManager"
                    target="_blank"
                    rel="noopener noreferrer"
                    sx={{color: 'white', mr: 1}}
                >
                  <GitHub/>
                </IconButton>
              </Box>
            </Grid>
          </Grid>

          <Divider sx={{my: 4, borderColor: 'rgba(255, 255, 255, 0.1)'}}/>

          {/* Bottom Section */}
          <Box
              sx={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                flexWrap: 'wrap',
              }}
          >
            <Typography variant="body2" sx={{opacity: 0.7}}>
              © {currentYear} UE Package Manager. All rights reserved.
            </Typography>
            <Typography variant="body2" sx={{opacity: 0.7}}>
              Made with ♥️ for the Unreal Engine Community
            </Typography>
          </Box>
        </Container>
      </Box>
  );
}