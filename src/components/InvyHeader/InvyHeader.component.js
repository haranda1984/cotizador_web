import styled from 'styled-components'

const InverlevyHeader = styled.div`
  font-weight: lighter;
  color: ${p => p.color ? p.color : 'white'};
  font-size: ${p => p.fontSize ? p.fontSize : '7em'}
`
export default InverlevyHeader
