import React from 'react'
import styled from 'styled-components'
import Loader from 'react-loader-spinner'

const StyledWrapper = styled.div`
display:flex;
align-items:center;
justify-content:center;
flex:1
`

const SuspenseFallBack = () => {
  return <StyledWrapper>
    <Loader type="ThreeDots" color="#000000"/>
  </StyledWrapper>
}
export default SuspenseFallBack
