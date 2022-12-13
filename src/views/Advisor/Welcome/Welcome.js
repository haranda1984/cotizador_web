import React, { useContext } from 'react'
import styled from 'styled-components'
import { InverlevyButton } from '../../../components/InvyButton/InvyButton.component'
import { navigate } from '@reach/router'
import InverlevyStyledWrapper from '../../../components/InvyWrapper/InvyWrapper.component'
import InverlevyHeader from '../../../components/InvyHeader/InvyHeader.component'
import {
  AGENT_BOSQUE_ROLE, AGENT_MOLINO_ROLE,AGENT_CIELO_ROLE,SUPERADMIN_ROLE, AGENT_ROLE, AGENT_EDIFICIO_ROLE, AGENT_VALLE_ROLE, AGENT_MONTANA_ROLE, PRIMARYCOLOR
} from '../../../constants/utils'
import { AuthContext } from '../../../providers/session/Auth.provider'

const StyledInfoWrapper = styled.div`
  width: 100%;
  height: ${(p) => (p.cut ? 'calc(100vh - 56px)' : '100vh')};
  display: inline-grid;
  grid-template-columns: 1fr 1fr;
  align-items: center;
  background-color: ${(p) =>
    p.backgroundColor ? p.backgroundColor : ' white'};
`

const StyledP = styled.p`
  color: ${PRIMARYCOLOR};
  font-size: 1.5em;
  font-weight: lighter;
`

const Welcome = () => {
  const { state } = useContext(AuthContext)
  return (
    <InverlevyStyledWrapper>
      {state.loggedIn && (state.roles.includes(SUPERADMIN_ROLE) || state.roles.includes(AGENT_VALLE_ROLE) || state.roles.includes(AGENT_ROLE)) && <StyledInfoWrapper backgroundColor={'#F1F3F5'}>
        <div style={{ width: '60%', margin: ' 0 auto' }}>
          <InverlevyHeader fontSize={'4em'} color={'#ff9619'}>
            Valle Escondido
          </InverlevyHeader>
          <StyledP>Un paraiso en la ciudad</StyledP>
          <InverlevyButton
            backgroundColor={PRIMARYCOLOR}
            borderColor={PRIMARYCOLOR}
            onClick={() =>
              navigate('/quote?product=EB04A05E-0408-4D9B-9B27-71A6C8EE1B4D')
            }
          >
            Cotizar
          </InverlevyButton>
        </div>
        <div style={{ display: 'flex', justifyContent: 'flex-start' }}>
          <img
            style={{ borderRadius: '5px', width: '70%' }}
            src={'https://heicommunityquotes.blob.core.windows.net/realtor/main/eb04a05e-0408-4d9b-9b27-71a6c8ee1b4d.jpg'}
            alt="Valle"
          />
        </div>
      </StyledInfoWrapper>}

      {state.loggedIn && (state.roles.includes(SUPERADMIN_ROLE) || state.roles.includes(AGENT_BOSQUE_ROLE) || state.roles.includes(AGENT_ROLE)) && <StyledInfoWrapper>
        <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
          <img
            style={{ borderRadius: '5px', width: '70%' }}
            src={'https://heicommunityquotes.blob.core.windows.net/realtor/main/831d051c-f5a5-40f4-929e-6a7fc4470caa.jpg'}
            alt="Bosque"
          />
        </div>
        <div style={{ width: '60%', margin: ' 0 auto' }}>
          <InverlevyHeader fontSize={'4em'} color={'#D6A285'}>
            Bosque Real
          </InverlevyHeader>
          <StyledP>El escape de la ciudad perfecto</StyledP>
          <InverlevyButton
            backgroundColor={PRIMARYCOLOR}
            borderColor={PRIMARYCOLOR}
            onClick={() =>
              navigate('/quote?product=831D051C-F5A5-40F4-929E-6A7FC4470CAA')
            }
          >
            Cotizar
          </InverlevyButton>
        </div>
      </StyledInfoWrapper>}

      {state.loggedIn && (state.roles.includes(SUPERADMIN_ROLE) || state.roles.includes(AGENT_EDIFICIO_ROLE) || state.roles.includes(AGENT_ROLE)) && <StyledInfoWrapper backgroundColor={'#F1F3F5'}>
        <div style={{ width: '60%', margin: ' 0 auto' }}>
          <InverlevyHeader fontSize={'4em'} color={'#442464'}>
            Edificio Nueva España
          </InverlevyHeader>
          <StyledP>La funsion del viejo y nuevo mundo</StyledP>
          <InverlevyButton
            onClick={() =>
              navigate('/quote?product=C4A01BEE-D182-4CAA-84D4-7BB5F016BD00')
            }
          >
            Cotizar
          </InverlevyButton>
        </div>
        <div style={{ display: 'flex', justifyContent: 'flex-start' }}>
          <img
            style={{ borderRadius: '5px', width: '70%' }}
            src={'https://heicommunityquotes.blob.core.windows.net/realtor/main/c4a01bee-d182-4caa-84d4-7bb5f016bd00.jpg'}
            alt="Edificio"
          />
        </div>
      </StyledInfoWrapper>}

      {state.loggedIn && (state.roles.includes(SUPERADMIN_ROLE) || state.roles.includes(AGENT_CIELO_ROLE) || state.roles.includes(AGENT_ROLE)) && <StyledInfoWrapper>
        <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
          <img
            style={{ borderRadius: '5px', width: '70%' }}
            src={'https://heicommunityquotes.blob.core.windows.net/realtor/main/6776169d-8b31-45b2-8abb-e3c218c086d1.jpg'}
            alt="Cielo"
          />
        </div>
        <div style={{ width: '60%', margin: ' 0 auto' }}>
          <InverlevyHeader fontSize={'4em'} color={'#003f44'}>
            Cielo Estrellado
          </InverlevyHeader>
          <StyledP>Tu hogar en las nubes</StyledP>
          <InverlevyButton
            onClick={() =>
              navigate('/quote?product=6776169d-8b31-45b2-8abb-e3c218c086d1')
            }
          >
            Cotizar
          </InverlevyButton>
        </div>
      </StyledInfoWrapper>}

      {state.loggedIn && (state.roles.includes(SUPERADMIN_ROLE) || state.roles.includes(AGENT_MOLINO_ROLE) || state.roles.includes(AGENT_ROLE)) && <StyledInfoWrapper backgroundColor={'#F1F3F5'}>
        <div style={{ width: '60%', margin: ' 0 auto' }}>
          <InverlevyHeader fontSize={'4em'} color={'#ff9619'}>
            Molino San Sebastian
          </InverlevyHeader>
          <StyledP>Fluye en tu entorno ideal</StyledP>
          <InverlevyButton
            backgroundColor={PRIMARYCOLOR}
            borderColor={PRIMARYCOLOR}
            onClick={() =>
              navigate('/quote?product=9a36c2b1-243c-4e6e-881b-2757a4b06aec')
            }
          >
            Cotizar
          </InverlevyButton>
        </div>
        <div style={{ display: 'flex', justifyContent: 'flex-start' }}>
          <img
            style={{ borderRadius: '5px', width: '70%' }}
            src={'https://heicommunityquotes.blob.core.windows.net/realtor/main/9a36c2b1-243c-4e6e-881b-2757a4b06aec.jpg'}
            alt="Molino"
          />
        </div>
      </StyledInfoWrapper>}

      {state.loggedIn && (state.roles.includes(SUPERADMIN_ROLE) || state.roles.includes(AGENT_MONTANA_ROLE) || state.roles.includes(AGENT_ROLE)) && <StyledInfoWrapper>
        <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
          <img
            style={{ borderRadius: '5px', width: '70%' }}
            src={'https://heicommunityquotes.blob.core.windows.net/realtor/main/7ce1e2f0-fff7-41c6-aa4a-e12ee4c0d1ca.jpg'}
            alt="Montaña"
          />
        </div>
        <div style={{ width: '60%', margin: ' 0 auto' }}>
          <InverlevyHeader fontSize={'4em'} color={'#104358'}>
            Montaña Desierta
          </InverlevyHeader>
          <StyledP>La casa de tus sueños</StyledP>
          <InverlevyButton
            onClick={() =>
              navigate('/quote?product=7CE1E2F0-FFF7-41C6-AA4A-E12EE4C0D1CA')
            }
          >
            Cotizar
          </InvyButton>
        </div>
      </StyledInfoWrapper>}

    </InverlevyStyledWrapper>
  )
}

export default Welcome
