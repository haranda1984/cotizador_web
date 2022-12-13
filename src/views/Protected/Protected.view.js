import React, { useContext, Suspense } from 'react'
import styled from 'styled-components'
import axios from 'axios'
import JwtDecode from 'jwt-decode'

import { AuthContext } from '../../providers/session/Auth.provider'
import { AuthActions } from '../../providers/session/Auth.reducer'
import { Redirect } from '@reach/router'
import SuspenseFallBack from '../../components/SuspenseFallBack/SuspenseFallBack.component'
import Header from '../../components/Header/Header.component'
import Footer from '../../components/Footer/Footer.component'

const Wrapper = styled.div`
  min-height: 100%;
  display: grid;
  grid-template-rows: auto 1fr auto;
`

// eslint-disable-next-line react/prop-types
const ProtectedView = ({ as: View, roles, ...props }) => {
  const { state, dispatch } = useContext(AuthContext)
  if (state.loggedIn && state.user && new Date() < new Date(state.expiresIn)) {
    // logged
  } else if (JSON.parse(localStorage.getItem('userData')) && new Date() < new Date(JSON.parse(localStorage.getItem('expiresIn')))) {
    axios.defaults.headers.common.Authorization = 'Bearer ' + JSON.parse(localStorage.getItem('userData'))
   
    setTimeout(() => {
      dispatch({ type: AuthActions.LOGIN, data: { loggedIn: true, user: JSON.parse(localStorage.getItem('userData')), roles: JwtDecode(JSON.parse(localStorage.getItem('userData'))).roles, expiresIn: JSON.parse(localStorage.getItem('expiresIn')), params: JSON.parse(localStorage.getItem('params')) } })
    })
  } else {
    localStorage.removeItem('userData')
    localStorage.removeItem('expiresIn')
    localStorage.removeItem('params')
    setTimeout(() => {
      dispatch({ type: AuthActions.LOGOUT })
    })
    return <Redirect noThrow to="/" />
  }

  if (roles && state.roles && !roles.includes(state.roles)) {
    return <Redirect noThrow to="/404" />
  }
  return (
    <React.Fragment>
      <Wrapper>
        <Header/>
        <div style={{ backgroundColor: '#F1F3F5' }}>
          <Suspense fallback={<SuspenseFallBack/>}>
            <View {...props}/>
          </Suspense>
        </div>
        <Footer/>
      </Wrapper>
    </React.Fragment>
  )
}

export default ProtectedView
