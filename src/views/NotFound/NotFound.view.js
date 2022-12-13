import React from 'react'
import styled from 'styled-components'
import Header from '../../components/Header/Header.component'
import Footer from '../../components/Footer/Footer.component'
import { Box, Paper } from '@material-ui/core'
import { PRIMARYCOLOR } from '../../constants/utils'
import { InverlevyButton } from '../../components/InverlevyButton/InverlevyButton.component'
import { navigate } from '@reach/router'

const Wrapper = styled.div`
  min-height: 100%;
  display: grid;
  grid-template-rows: auto 1fr auto;
`

const NotFound = () => (<Wrapper>
  <Header/>
  <div style={{
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#F1F3F5'
  }}>
    <Paper elevation={3} >
      <Box p={3} m={3} style={{ display: 'grid', gridTemplateColumns: '3fr 2fr' }}>
        <img src={`${process.env.PUBLIC_URL}/assets/images/404.svg`} alt="no-encontrado"/>
        <Box p={2} color={PRIMARYCOLOR} style={{ display: 'flex', flexFlow: 'column', alignItems: 'center', justifyContent: 'center' }}>
          <div>
            <h1 style={{ marginBottom: 0 }}>Lo que buscas,</h1>
            <h1 style={{ marginTop: 0 }}>está en otro lugar.</h1>
            <InverlevyButton onClick={() => navigate(-1)}>Volver</InverlevyButton>
          </div>
        </Box>
      </Box>
    </Paper>
  </div>
  <Footer/>
</Wrapper>)

export default NotFound
