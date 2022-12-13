import { navigate } from '@reach/router'
import React from 'react'
import styled from 'styled-components'
import { InverlevyButton } from '../../components/InvyButton/InvyButton.component'
import InverlevyHeader from '../../components/InvyHeader/InvyHeader.component'

const StyledBeginWrapper = styled.div`
  display: flex;
  align-items: end;
  justify-content:center;
  flex-flow:column;
  height:100%;
  width: 100%;
  background-image: url("${process.env.PUBLIC_URL}/assets/images/hero-volver.jpg");
  background-position: center; 
  background-repeat: no-repeat; 
  background-size: cover;
`

const GoBack = () => {
  return <StyledBeginWrapper>
    <div style={{ paddingLeft: '10%' }}>
      <InverlevyHeader fontSize={'6em'} >¿Aún dudas</InverlevyHeader>
      <InverlevyHeader fontSize={'6em'} >de tu inversión?</InverlevyHeader>
      <InvyButton backgroundColor={'black'} onClick={() => navigate('begin')}>VOLVER A COTIZAR</InvyButton>
    </div>
  </StyledBeginWrapper>
}
export default GoBack
