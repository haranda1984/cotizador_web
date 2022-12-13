import React, { useState, useContext } from "react";
import styled from "styled-components";
import { navigate, Redirect } from "@reach/router";
import axios from "axios";
import JwtDecode from "jwt-decode";
import { Formik, Field, Form } from "formik";
import * as Yup from "yup";
import * as moment from 'moment'
import endpoints from "../../api-service/endpoints";

import { InverlevyLoginButton } from "../../components/InvyLoginButton/InvyLoginButton.component";
import Loader from "react-loader-spinner";
import { AuthContext } from "../../providers/session/Auth.provider";
import { AuthActions } from "../../providers/session/Auth.reducer";
import { PRIMARYCOLOR } from "../../constants/utils";

const StyledLanding = styled.div`
  display: flex;
  flex-flow: column;
  justify-content: center;
  align-items: center;
  background-color: #f4f5f9;
  background-image: url("${process.env
    .PUBLIC_URL}/assets/images/background.webp");
  background-position: center;
  background-repeat: no-repeat;
  background-size: cover;
  height: 100%;
`;

const StyledHeader = styled.header`
  display: grid;
  padding: 10px 0px;
  position: absolute;
  top: 0;
  align-items:center;
  width: 100%;
  padding: 10px 0;
  background-color: #ffffff;
    > img {
      cursor: pointer;
      height: 35px;
      padding: 0 40px;
    }  
`

const StyledFooter = styled.div`
  display: flex;
  padding: 10px 0;
  align-items: center;
  justify-content: space-between;
  height: 2em;
  color:#a80e0e;
  position: absolute;
  bottom: 0;
  width: 100%;
  background-color: #ffff;
`
const StyledSpan = styled.span`
  padding: 0 40px;
  color: #666767;
    > a {
      text-decoration: none;
      color: #666767;
    }
`

const StyledWrapper = styled.div`
  display: flex;
  flex-flow: column;
  align-items: center;
  justify-content: center;
  background: white;
  padding: 3rem;
  box-sizing: border-box;
  box-shadow: 0px 4px 4px rgba(0, 0, 0, 0.25);
`;

const StyledForm = styled(Form)`
  display: flex;
  flex-flow: column;
  justify-content: center;
  align-items: center;
  width: 24em;
`;

const StyledLandingInput = styled(Field)`
  margin: 1em;
  font-size: 1em;
  height: 3em;
  width: 20em;
  border-radius: 5px;
  border: 1px solid lightgrey;

  ::-webkit-input-placeholder {
    text-align: center;
    color: grey;
  }

  :-moz-placeholder {
    /* Firefox 18- */
    text-align: center;
    color: grey;
  }

  ::-moz-placeholder {
    /* Firefox 19+ */
    text-align: center;
    color: grey;
  }

  :-ms-input-placeholder {
    text-align: center;
    color: grey;
  }
`;
const StyledError = styled.div`
  color: red;
  margin-top: -10px;
`;

const SignupSchema = Yup.object().shape({
  email: Yup.string()
    .email("Correo electronico invalido")
    .required("Requerido"),
  password: Yup.string().required("Requerido"),
});

const SetLogin = (data, now) => {
  localStorage.removeItem("userData");
  localStorage.removeItem("expiresIn");
  localStorage.setItem("userData", JSON.stringify(data.accessToken));
  localStorage.setItem("expiresIn", JSON.stringify(now));
  axios.defaults.headers.common.Authorization = "Bearer " + data.accessToken;
};

// const SetParams = (params) => {
//   localStorage.removeItem('params')
//   localStorage.setItem('params', JSON.stringify(params))
// }

const Landing = (props) => {
  const [invalid, setinvalid] = useState(false);
  const { state, dispatch } = useContext(AuthContext);

  if (state.loggedIn && state.user && new Date() < new Date(state.expiresIn)) {
    return <Redirect noThrow to="/begin" />;
  } else if (
    JSON.parse(localStorage.getItem("userData")) &&
    new Date() < new Date(JSON.parse(localStorage.getItem("expiresIn")))
  ) {
    axios.defaults.headers.common.Authorization =
      "Bearer " + JSON.parse(localStorage.getItem("userData"));
   
    setTimeout(() => {
      // dispatch({ type: AuthActions.LOGIN, data: { loggedIn: true, user: JSON.parse(localStorage.getItem('userData')), roles: JwtDecode(JSON.parse(localStorage.getItem('userData'))).roles, expiresIn: JSON.parse(localStorage.getItem('expiresIn')), params: JSON.parse(localStorage.getItem('params')) } })
      dispatch({
        type: AuthActions.LOGIN,
        data: {
          loggedIn: true,
          user: JSON.parse(localStorage.getItem("userData")),
          roles: JwtDecode(JSON.parse(localStorage.getItem("userData"))).roles,
          expiresIn: JSON.parse(localStorage.getItem("expiresIn")),
        },
      });
      return <Redirect noThrow to="/begin" />;
    });
  }

  return (
    <StyledLanding>
      <StyledHeader>
        <img
          src={`${process.env.PUBLIC_URL}/assets/images/crwb.png`}
          alt="Cotizador Web"
          onClick={() => navigate("/")}
        />
      </StyledHeader>
      <StyledWrapper>
        <h1 style={{ color: `#1365EC` }}>Bienvenido</h1>
        <Formik
          initialValues={{
            email: "",
            password: "",
          }}
          validationSchema={SignupSchema}
          onSubmit={async (values) => {
            try {
              setinvalid(false);
              const { data } = await axios.post(endpoints.LOGIN, {
                username: values.email,
                password: values.password,
              });
              // check expiration in sepate stuff
              var now = new Date();
              now.setSeconds(now.getSeconds() + data.expiresIn);
              SetLogin(data, now);
              // const { data: params } = await axios.get(endpoints.PARAMS)
              // SetParams(params)
              // dispatch({ type: AuthActions.LOGIN, data: { loggedIn: true, user: data.accessToken, roles: JwtDecode(data.accessToken).roles, expiresIn: now, params } })
              dispatch({
                type: AuthActions.LOGIN,
                data: {
                  loggedIn: true,
                  user: data.accessToken,
                  roles: JwtDecode(data.accessToken).roles,
                  expiresIn: now,
                },
              });
              navigate("/begin");
            } catch (error) {
              console.log(error);
              setinvalid(true);
            }
          }}
        >
          {({ errors, touched, isSubmitting }) => (
            <StyledForm>
              <StyledLandingInput
                name="email"
                type="email"
                placeholder="Correo"
              />
              {errors.email && touched.email ? (
                <StyledError>{errors.email}</StyledError>
              ) : null}
              <StyledLandingInput
                name="password"
                type="password"
                placeholder="Contraseña"
              />
              {errors.password && touched.password ? (
                <StyledError>{errors.password}</StyledError>
              ) : null}
              {invalid && (
                <StyledError style={{ margin: 10 }}>
                  Usuario y/o contraseña inválido
                </StyledError>
              )}
              {isSubmitting ? (
                <Loader type="ThreeDots" color={PRIMARYCOLOR} />
              ) : (
                <InverlevyLoginButton type="submit" disabled={isSubmitting}>
                  Iniciar Sesión
                </InverlevyLoginButton>
              )}
            </StyledForm>
          )}
        </Formik>
      </StyledWrapper>
      <StyledFooter>
        <StyledSpan>© {moment().year()} Cotizador Web</StyledSpan>
        <StyledSpan><a href="https://rootinc.mx/terms.html" target="_blank">Legal</a> | <a href="https://rootinc.mx/privacy.html" target="_blank">Aviso de privacidad</a> | <a href="https://rootinc.mx" target="_blank">Más información</a></StyledSpan>
      </StyledFooter>
    </StyledLanding>
  );
};

export default Landing;
