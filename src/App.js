import React from "react";
import { Router } from "@reach/router";
import styled from "styled-components";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import AuthProvider from "./providers/session/Auth.provider";

import { MuiPickersUtilsProvider } from "@material-ui/pickers";
import MomentUtils from "@date-io/moment";
import "moment/locale/es";

import Landing from "./views/Landing/Landing.view";
import Welcome from "./views/Advisor/Welcome/Welcome";
import NotFound from "./views/NotFound/NotFound.view";
import ProtectedView from "./views/Protected/Protected.view";
import Quote from "./views/Quote/Quote.view";
import Details from "./views/Details";
import GoBack from "./views/GoBack/GoBack.view";

// Admin
import Admin from "./views/Admin/Landing";

import "./App.css";
import "react-loader-spinner/dist/loader/css/react-spinner-loader.css";
import {
  SUPERADMIN_ROLE,
  ADMIN_ROLE,
  PRIMARYCOLOR,
  PRIMARYBLUE,
} from "./constants/utils";

const RouterStyled = styled(Router)`
  display: flex;
  flex-flow: column;
  flex: 1;
  height: 100%;
`;
const theme = createMuiTheme({
  palette: {
    primary: {
      main: PRIMARYCOLOR,
    },
    secondary: {
      main: PRIMARYBLUE,
    },
  },
});

function App() {
  return (
    <AuthProvider>
      <MuiPickersUtilsProvider utils={MomentUtils} locale={"es"}>
        <ThemeProvider theme={theme}>
          <RouterStyled>
            <Landing path="/" />
            <ProtectedView path="/quote" as={Quote} />
            <ProtectedView path="/quote-details" as={Details} />
            <ProtectedView path="/begin" as={Welcome} />
            <ProtectedView path="/review" as={GoBack} />
            <ProtectedView
              roles={[SUPERADMIN_ROLE, ADMIN_ROLE]}
              path="/admin"
              as={Admin}
            />
            <NotFound path="/404" default />
          </RouterStyled>
        </ThemeProvider>
      </MuiPickersUtilsProvider>
    </AuthProvider>
  );
}

export default App;
