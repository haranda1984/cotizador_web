import styled, { css } from 'styled-components'

const InverlevyStyledWrapper = styled.div`
  height: 100%;
  display: flex;
  align-items: center;
  justify-content:center;
  flex-flow:column;
  ${p => p.backgroundImage && BackgroundWrapper}
  `

const BackgroundWrapper = css`
  background-image: url("${process.env.PUBLIC_URL}/assets/images/${p => p.backgroundImage}");
  background-position: center; 
  background-repeat: no-repeat; 
  background-size: cover;
`

export default InverlevyStyledWrapper
