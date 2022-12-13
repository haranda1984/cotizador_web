import styled from 'styled-components'
import { PRIMARYCOLOR } from '../../constants/utils'

const StyledPaper = styled.div`
display: flex; 
height: 65vh; 
background-color: white; 
flex-flow: column;
align-items: center;
color: ${PRIMARYCOLOR};
width: ${p => p.width ? p.width : '70%'};
justify-content: center;
overflow: hidden;
  > div {
    width: 90%;
  }
`

export default StyledPaper
