import React from 'react'
import styled from 'styled-components'
import * as moment from 'moment'

const StyledFooter = styled.div`
display: flex;
padding:12px;
grid-template-columns: 1fr 1fr;
align-items: center;
justify-content: space-between;
height: 2em;
color:#a80e0e;
`
const StyledSpan = styled.span`
padding:12px;
color: grey;
`

const Footer = () => (
  <StyledFooter>
    <StyledSpan>© {moment().year()} Root Inc</StyledSpan>
        <StyledSpan><a href="https://rootinc.mx/terms.html" target="_blank">Legal</a> | <a href="https://rootinc.mx/privacy.html" target="_blank">Aviso de privacidad</a> | <a href="https://rootinc.mx" target="_blank">Más información</a></StyledSpan>
  </StyledFooter>
)

export default Footer
