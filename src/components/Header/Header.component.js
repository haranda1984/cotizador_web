import React, { useContext } from 'react'
import styled, { css } from 'styled-components'
import { useLocation, Redirect, navigate } from '@reach/router'

import { InvyButton } from '../InvyButton/InvyButton.component'

import { AuthContext } from '../../providers/session/Auth.provider'
import { AuthActions } from '../../providers/session/Auth.reducer'
import { SUPERADMIN_ROLE, ADMIN_ROLE, PRIMARYCOLOR} from '../../constants/utils'

const StyledHeader = styled.header`
display: grid;
padding: 10px 40px;
grid-template-columns: 1fr 2fr 1fr;
align-items:center;
  > img {
    cursor: pointer;
    height: 35px;
  }
`
const StyledButtonContainer = styled.div`
display:flex;
justify-content: flex-end
`

const StyledContainer = styled.div`
display:flex;
justify-content: center;
`
const StyledTab = styled.span`
cursor: pointer;
margin: 1em;
color: grey;
${p => p.active && activeTab}
`

const activeTab = css`
  border-bottom: solid 3px ${PRIMARYCOLOR};
  margin-bottom: -10px;
  color: black;
`

const Header = () => {
  const location = useLocation()
  const { state, dispatch } = useContext(AuthContext)

  const onClick = () => {
    localStorage.removeItem('userData')
    localStorage.removeItem('expiresIn')
    localStorage.removeItem('params')
    dispatch({ type: AuthActions.LOGOUT })
    return <Redirect noThrow to="/" />
  }
  return <StyledHeader>
    <img src={`${process.env.PUBLIC_URL}/assets/images/crwb.png`} alt="Cotizador web" onClick={() => navigate('/')} /> //imagen marca
    <StyledContainer> {state.loggedIn &&
      <React.Fragment>
        <StyledTab id='tabProducts' onClick={() => navigate('/begin')} active={location.pathname.includes('begin')}>Proyectos</StyledTab>
        {state.loggedIn && (state.roles === SUPERADMIN_ROLE || state.roles === ADMIN_ROLE) && <>
          <StyledTab id='tabAdmin' onClick={() => navigate('/admin')} active={location.pathname.includes('admin')}>Admin</StyledTab>
        </>}
      </React.Fragment>}
    </StyledContainer>
    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'flex-end' }}>
      <StyledButtonContainer> {state.loggedIn && <InverlevyButton primary onClick={onClick}>Salir</InverlevyButton>} </StyledButtonContainer>
    </div>
  </StyledHeader>
}

export default Header
